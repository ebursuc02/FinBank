namespace Infrastructure.Options;

public class ConnectionOptions
{
    public string KYC { get; set; } = string.Empty;
}

public class PersistenceOptions
{
    public int CommandTimeoutSeconds { get; set; } = 30;
    public bool EnableRetryOnFailure { get; set; } = true;
    public int MaxRetryCount { get; set; } = 3;
    public int MaxRetryDelaySeconds { get; set; } = 5;
    public bool EnableSensitiveDataLogging { get; set; } = false;
    public bool UseDetailedErrors { get; set; } = false;
    public bool UseSensitiveDataLogging { get; set; } = false;
}