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

    internal static void Write(object o)
    {
        try
        {
            System.IO.File.AppendAllText(@"C:\logs\DebugWrite.log", o + "\n");
        }
        catch
        {
            Thread.Sleep(10);
            try
            {
                System.IO.File.AppendAllText(@"C:\logs\DebugWrite.log", o + "\n");
            }
            catch
            {

            }
        }
    }
}
