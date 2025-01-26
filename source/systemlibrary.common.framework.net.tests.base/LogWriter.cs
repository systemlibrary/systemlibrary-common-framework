namespace SystemLibrary.Common.Framework.Tests;

internal class LogWriter : ILogWriter
{
    public void Error(string message)
    {
        File.AppendAllText(@"C:\logs\log.log", message);
    }

    public void Warning(string message)
    {
        File.AppendAllText(@"C:\logs\log.log", message);
    }

    public void Debug(string message)
    {
        File.AppendAllText(@"C:\logs\log.log", message);
    }

    public void Information(string message)
    {
        File.AppendAllText(@"C:\logs\log.log", message);
    }

    public void Trace(string message)
    {
        File.AppendAllText(@"C:\logs\log.log", message);
    }

    public void Write(string message)
    {
        File.AppendAllText(@"C:\logs\log.log", message);
    }
}
