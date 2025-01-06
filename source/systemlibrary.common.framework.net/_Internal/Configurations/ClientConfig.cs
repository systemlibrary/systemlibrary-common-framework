namespace SystemLibrary.Common.Framework;

internal class ClientConfig
{
    public int Timeout { get; set; } = 40001;
    public int RetryTimeout { get; set; } = 10000;
    public bool IgnoreSslErrors { get; set; } = true;
    public bool UseRetryPolicy { get; set; } = true;
    public int ClientCacheDuration { get; set; } = 1200; // 20 minutes
    public bool ThrowOnUnsuccessful { get; set; } = true;
    public bool UseRequestBreakerPolicy { get; set; } = false;
}
