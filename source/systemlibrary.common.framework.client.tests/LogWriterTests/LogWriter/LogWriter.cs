namespace SystemLibrary.Common.Framework.App.Tests;

public class LogWriter : ILogWriter
{
    public void Error(string message)
    {
        Dump.Write("Error in LogWriter: " + message);
    }

    public void Warning(string message)
    {
        Dump.Write("Warning in LogWriter: " + message);
    }

    public void Debug(string message)
    {
        Dump.Write("Debug in LogWriter: " + message);
    }

    public void Information(string message)
    {
        Dump.Write("Info in LogWriter: " + message);
    }

    public void Trace(string message)
    {
        Dump.Write("Trace in LogWriter: " + message);
    }

    public void Write(string message)
    {
        Dump.Write("Write in LogWriter: " + message);
    }
}
