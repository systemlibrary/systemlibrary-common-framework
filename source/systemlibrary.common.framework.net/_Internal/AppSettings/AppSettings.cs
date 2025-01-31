namespace SystemLibrary.Common.Framework;

internal class AppSettings : Config<AppSettings>
{
    public AppSettings()
    {
        Logging = new LoggingConfig();
        DataProtection = new DataProtection();
        SystemLibraryCommonFramework = new FrameworkConfig();
    }

    public LoggingConfig Logging { get; set; }

    public DataProtection DataProtection { get; set; }

    public FrameworkConfig SystemLibraryCommonFramework { get; set; }
}


