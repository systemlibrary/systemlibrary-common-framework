namespace SystemLibrary.Common.Framework;

internal static class Debug
{
    static bool? _Debugging;

    static bool Debugging
    {
        get
        {
            _Debugging ??= FrameworkConfigInstance.Current?.Debug == true;

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
            System.IO.File.AppendAllText(@"C:\logs\DebugWrite.log", DateTime.Now.ToString() + "\t" +   o + "\n");
        }
        catch
        {
            Thread.Sleep(8);
            try
            {
                System.IO.File.AppendAllText(@"C:\logs\DebugWrite.log", DateTime.Now.ToString() + "\t" + o + "\n");
            }
            catch
            {
                try
                {
                    Thread.Sleep(8);
                    System.IO.File.AppendAllText(@"C:\logs\DebugWrite.log", DateTime.Now.ToString() + "\t" + o + "\n");
                }
                catch
                {
                }
            }
        }
    }
}
