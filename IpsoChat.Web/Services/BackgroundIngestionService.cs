using IpsoChat.Web.Services.Ingestion;


namespace IpsoChat.Web.Services;

public class BackgroundIngestionService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<BackgroundIngestionService> _logger;
    private readonly string _dataDirectory;

    public BackgroundIngestionService(
        IServiceProvider serviceProvider, 
        ILogger<BackgroundIngestionService> logger,

        IWebHostEnvironment environment)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _dataDirectory = Path.Combine(environment.WebRootPath, "Data");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Background ingestion service started");
        
        try
        {
            await DataIngestor.IngestDataAsync(
                _serviceProvider,
                new PDFDirectorySource(_dataDirectory));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during background ingestion");
        }
        
        _logger.LogInformation("Background ingestion service completed");
    }
}