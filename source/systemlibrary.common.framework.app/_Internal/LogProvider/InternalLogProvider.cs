using Microsoft.Extensions.Logging;

namespace SystemLibrary.Common.Framework;

internal class InternalLogProvider : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName)
    {
        return new InternalLog();
    }

    public void Dispose()
    {
    }
}
