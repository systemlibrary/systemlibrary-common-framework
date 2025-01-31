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
        System.IO.File.AppendAllText(@"C:\logs\write.log", o + "\n");
    }
}
