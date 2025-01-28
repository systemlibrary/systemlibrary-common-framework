namespace SystemLibrary.Common.Framework.Tests;

internal class LogWriter : ILogWriter
{
    public void Critical(string message)
    {
        Log.Dump(message);
    }

    public void Error(string message)
    {
        Log.Dump(message);
    }

    public void Warning(string message)
    {
        Log.Dump(message);
    }

    public void Debug(string message)
    {
        Log.Dump(message);
    }

    public void Information(string message)
    {
        Log.Dump(message);
    }

    public void Trace(string message)
    {
        Log.Dump(message);
    }

    public void Write(string message)
    {
        Log.Dump(message);
    }
}
