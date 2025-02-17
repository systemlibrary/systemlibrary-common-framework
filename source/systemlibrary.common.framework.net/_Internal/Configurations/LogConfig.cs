namespace SystemLibrary.Common.Framework;

internal class LogConfig
{
    public string FullFilePath { get; set; }

    public Log.LogLevel Level { get; set; } = Log.LogLevel.Warning;
    public Log.LogFormat Format { get; set; } = Log.LogFormat.Text;

    public bool AddUrl { get; set; } = true;
    public bool AddHttpMethod { get; set; } = false;
    public bool AddAuthenticatedState { get; set; } = false;
    public bool AddStacktrace { get; set; } = false;
    public bool AddIP { get; set; } = false;
    public bool AddBrowserName { get; set; } = false;
    public bool AddCorrelationId { get; set; } = true;

    string _FullFilePath;
    internal string GetFullFilePath()
    {
        if (_FullFilePath == null)
        {
            var tmp = FullFilePath;

            if (tmp.IsNot())
            {
                tmp = "%HomeDrive%/Logs/Dump.log";
            }

            try
            {
                tmp = Environment.ExpandEnvironmentVariables(tmp);
            }
            catch
            {
                // Swallow errors to avoid breaking in uncertain environments
            }

            if (tmp.Contains("%"))
            {
                try
                {
                    tmp = tmp.Replace("%HomeDrive%", Environment.GetLogicalDrives()[0], StringComparison.OrdinalIgnoreCase);
                }
                catch
                {
                    tmp = tmp.Replace("%HomeDrive%", "/", StringComparison.OrdinalIgnoreCase);
                }
            }

            tmp = tmp.Replace("\\", "/");
            tmp = tmp.Replace("//", "/");

            _FullFilePath = tmp;
        }
        return _FullFilePath;
    }
}

