using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;

namespace IpsoChat.Web.Services.Ingestion;

public class DataIngestor(
    ILogger<DataIngestor> logger,
    VectorStoreCollection<Guid, IngestedChunk> chunksCollection,
    VectorStoreCollection<Guid, IngestedDocument> documentsCollection,
    IngestionProgressService progressService)
{
    public static async Task IngestDataAsync(IServiceProvider services, IIngestionSource source)
    {
        using var scope = services.CreateScope();
        var ingestor = scope.ServiceProvider.GetRequiredService<DataIngestor>();
        await ingestor.IngestDataAsync(source);
    }

    public async Task IngestDataAsync(IIngestionSource source)
    {
        await chunksCollection.EnsureCollectionExistsAsync();
        await documentsCollection.EnsureCollectionExistsAsync();

        var sourceId = source.SourceId;
        var documentsForSource = await documentsCollection.GetAsync(doc => doc.SourceId == sourceId, top: int.MaxValue).ToListAsync();

        var deletedDocuments = await source.GetDeletedDocumentsAsync(documentsForSource);
        foreach (var deletedDocument in deletedDocuments)
        {
            logger.LogInformation("Removing ingested data for {documentId}", deletedDocument.DocumentId);
            await DeleteChunksForDocumentAsync(deletedDocument);
            await documentsCollection.DeleteAsync(deletedDocument.Key);
        }

        var modifiedDocuments = await source.GetNewOrModifiedDocumentsAsync(documentsForSource);
        var totalDocuments = modifiedDocuments.Count();
        progressService.UpdateProgress(totalDocuments, 0);

        var processed = 0;
        foreach (var modifiedDocument in modifiedDocuments)
        {
            try
            {
                logger.LogInformation("Processing {documentId}", modifiedDocument.DocumentId);
                await DeleteChunksForDocumentAsync(modifiedDocument);

                await documentsCollection.UpsertAsync(modifiedDocument);

                var newRecords = await source.CreateChunksForDocumentAsync(modifiedDocument);
                await chunksCollection.UpsertAsync(newRecords);
            }
            catch (Exception ex)
            { 
                logger.LogError(ex, "Error processing {documentId}, rolling back", modifiedDocument.DocumentId);
                await documentsCollection.DeleteAsync(modifiedDocument.Key);
                await DeleteChunksForDocumentAsync(modifiedDocument);
            }
            try
            {
                processed++;
                progressService.UpdateProgress(totalDocuments, processed);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error Updating Data {documentId}", modifiedDocument.DocumentId);
            }

        }

        logger.LogInformation("Ingestion is up-to-date");
        progressService.UpdateProgress(totalDocuments, totalDocuments);

        async Task DeleteChunksForDocumentAsync(IngestedDocument document)
        {
            var documentId = document.DocumentId;
            var chunksToDelete = await chunksCollection.GetAsync(record => record.DocumentId == documentId, int.MaxValue).ToListAsync();
            if (chunksToDelete.Any())
            {
                await chunksCollection.DeleteAsync(chunksToDelete.Select(r => r.Key));
            }
        }
    }
}
