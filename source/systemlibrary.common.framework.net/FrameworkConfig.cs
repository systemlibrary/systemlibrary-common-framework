namespace SystemLibrary.Common.Framework;

internal class FrameworkConfig
{
    public bool Debug { get; set; }
    public JsonConfig Json { get; set; }
    public LogConfig Log { get; set; }
    public CacheConfig Cache { get; set; }
    public ClientConfig Client { get; set; }
    public MetricsConfig Metrics { get; set; }

    public FrameworkConfig()
    {
        Debug = false;
        Json = new JsonConfig();
        Log = new LogConfig();
        Cache = new CacheConfig();
        Client = new ClientConfig();
        Metrics = new MetricsConfig();
    }

    internal static FrameworkConfig Current
    {
        get
        {   
            return AppSettings.Current.SystemLibraryCommonFramework;
        }
    }
}
