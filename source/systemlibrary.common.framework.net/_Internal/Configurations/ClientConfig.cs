namespace SystemLibrary.Common.Framework;

internal class ClientConfig
{
    public int Timeout { get; set; } = 40001;
    public int RetryTimeout { get; set; } = 10000;
    public int ClientCacheDuration { get; set; } = 1200; // 20 minutes, note: This should be hard-coded, not an option to configure, less is more

    public bool IgnoreSslErrors { get; set; } = true;
    public bool ThrowOnUnsuccessful { get; set; } = true;

    public bool UseRetryPolicy { get; set; } = true;
    public bool UseAutomaticDecompression { get; set; } = true;
    public bool ExpectContinue { get; set; } = false;
    public bool UseRequestBreakerPolicy { get; set; } = false;
}
