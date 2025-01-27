namespace SystemLibrary.Common.Framework;

internal static class Debug
{
    static bool? _Debugging;

    static bool Debugging
    {
        get
        {
            _Debugging ??= FrameworkConfig.Current?.Debug == true;

            return _Debugging.Value;
        }
    }

    internal static void Log(string msg)
    {
        if (Debugging)
        {
            global::Log.Debug("[systemLibraryCommonFramework.Debug=true] " + msg);
        }
    }

    internal static void Write(string obj)
    {
        try
        {
            File.AppendAllText(@"C:\Logs\debug.log", "Debug Write: " + obj + "\n");
        }
        catch
        {
            Thread.Sleep(33);
            try
            {
                File.AppendAllText(@"C:\Logs\debug.log", "Debug Write: " + obj + "\n");
            }
            catch
            {
            }
        }
    }
}
