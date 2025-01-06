namespace SystemLibrary.Common.Framework;

internal class LogConfig
{
    public Log.LogLevel? Level { get; set; }

    public bool AppendLoggedInState { get; set; } = true;
    public bool AppendIp { get; set; } = false;

    public bool AppendPath { get; set; } = true;
    public bool AppendBrowser { get; set; } = false;
    public bool AppendCorrelationId { get; set; } = true;
    public string Format { get; set; }  // json | null
}

