namespace SystemLibrary.Common.Framework;

internal class FrameworkConfig
{
    public string License { get; set; }
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
}

internal static class FrameworkConfigInstance
{
    static FrameworkConfig _Current;
    static object _CurrentLock = new object();
    internal static FrameworkConfig Current
    {
        get
        {
            if(_Current == null)
            {
                lock (_CurrentLock)
                {
                    if (_Current != null) return _Current;

                    _Current = AppSettings.Current.SystemLibraryCommonFramework;
                }
            }
            return _Current;
        }
    }
}
