using System.Collections;

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
        var m = o?.ToString();

        if (o is Array arr)
            o += " (" + arr?.Length + ")";

        else if (o is IList list)
            o += " (" + list?.Count + ")";

        try
        {
            System.IO.File.AppendAllText(@"C:\logs\DebugWrite.log", DateTime.Now.ToString() + "\t" + m + "\n");
        }
        catch
        {
            Thread.Sleep(6);
            try
            {
                System.IO.File.AppendAllText(@"C:\logs\DebugWrite.log", DateTime.Now.ToString() + "\t" + m + "\n");
            }
            catch
            {
                try
                {
                    Thread.Sleep(6);
                    System.IO.File.AppendAllText(@"C:\logs\DebugWrite.log", DateTime.Now.ToString() + "\t" + m + "\n");
                }
                catch
                {
                }
            }
        }
    }
}
