namespace SystemLibrary.Common.Framework;

internal class LogConfig
{
    public string FullFilePath { get; set; }

    public Log.LogLevel Level { get; set; }
    public Log.LogFormat Format { get; set; }

    public bool AddUrl { get; set; } = false;
    public bool AddHttpMethod { get; set; } = true;
    public bool AddAuthenticatedState { get; set; } = false;
    public bool AddStacktrace { get; set; } = false;
    public bool AddIP { get; set; } = true;
    public bool AddBrowserName { get; set; } = true;
    public bool AddCorrelationId { get; set; } = false;

    string _FullFilePath;
    public string GetFullFilePath()
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

