namespace SystemLibrary.Common.Framework;

internal class LoggingConfig
{
    public LoggingLogLevel LogLevel { get; set; }
    public LoggingConfig()
    {
        LogLevel = new LoggingLogLevel();
    }
}

internal class LoggingLogLevel
{
    public string Default { get; set; } = "Information";
}