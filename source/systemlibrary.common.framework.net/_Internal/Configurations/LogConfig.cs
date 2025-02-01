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

            var firstIndex = tmp.IndexOf('%');

            if (firstIndex > -1)
            {
                var lastIndex = tmp.LastIndexOf('%');

                if (firstIndex == lastIndex) throw new Exception("Log folder cannot contain only one %, specify for instance: %HomeDrive%");

                var varName = tmp.Substring(firstIndex + 1, lastIndex - firstIndex - 1);

                var value = "";

                try
                {
                    value = Environment.GetEnvironmentVariable(varName);
                }
                catch
                {
                }

                if (value.IsNot())
                {
                    try
                    {
                        value = Environment.GetEnvironmentVariable(varName, EnvironmentVariableTarget.Machine);
                    }
                    catch
                    {
                    }
                }

                if (value.IsNot() && varName.ToLower() == "homedrive")
                {
                    value = Environment.GetLogicalDrives()?[0];
                }

                tmp = tmp.Replace("%" + varName + "%", value);
            }

            _FullFilePath = tmp;
        }
        return _FullFilePath;
    }
}

