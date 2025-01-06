namespace SystemLibrary.Common.Framework;

internal class FrameworkConfig
{
    public bool Debug { get; set; }
    public DumpConfig Dump { get; set; }
    public JsonConfig Json { get; set; }
    public LogConfig Log { get; set; }
    public CacheConfig Cache { get; set; }
    public ClientConfig Client { get; set; }
    public MetricsConfig Metrics { get; set; }

    public FrameworkConfig()
    {
        Debug = false;
        Dump = new DumpConfig();
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
            if (AppSettings.Current == null)
                global::Dump.Write("NULLCURRENT");

            if (AppSettings.Current.SystemLibraryCommonFramework == null)
                global::Dump.Write("SystemLibraryCommonFrameworknull");

            return AppSettings.Current.SystemLibraryCommonFramework;
        }
    }
}
