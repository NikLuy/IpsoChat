namespace IpsoChat.Web.Services;

public class IngestionProgressService
{
    public event Action<IngestionProgress>? ProgressChanged;
    
    private IngestionProgress _currentProgress = new();
    
    public IngestionProgress CurrentProgress => _currentProgress;
    
    public void UpdateProgress(int totalDocuments, int processedDocuments, string? currentDocument = null)
    {
        _currentProgress = new IngestionProgress
        {
            TotalDocuments = totalDocuments,
            ProcessedDocuments = processedDocuments,
            CurrentDocument = currentDocument,
            IsCompleted = processedDocuments >= totalDocuments
        };
        
        ProgressChanged?.Invoke(_currentProgress);
    }
    
    public void Reset()
    {
        _currentProgress = new IngestionProgress();
        ProgressChanged?.Invoke(_currentProgress);
    }
}

public class IngestionProgress
{
    public int TotalDocuments { get; set; }
    public int ProcessedDocuments { get; set; }
    public string? CurrentDocument { get; set; }
    public bool IsCompleted { get; set; }
    public int RemainingDocuments => Math.Max(0, TotalDocuments - ProcessedDocuments);
    public double ProgressPercentage => TotalDocuments > 0 ? (double)ProcessedDocuments / TotalDocuments * 100 : 0;
}