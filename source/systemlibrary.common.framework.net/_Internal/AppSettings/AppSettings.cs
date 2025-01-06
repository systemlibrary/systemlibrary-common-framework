namespace SystemLibrary.Common.Framework;

internal class AppSettings : Config<AppSettings>
{
    public AppSettings()
    {
        SystemLibraryCommonFramework = new FrameworkConfig();
        Logging = new LoggingConfig();
        DataProtection = new DataProtection();
    }

    public FrameworkConfig SystemLibraryCommonFramework { get; set; }

    public LoggingConfig Logging { get; set; }
    public DataProtection DataProtection { get; set; }
}


