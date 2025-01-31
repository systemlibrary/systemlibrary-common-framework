using System;

using Microsoft.Extensions.Logging;

namespace SystemLibrary.Common.Framework;

internal class InternalLog : ILogger
{
    public IDisposable BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }

    public bool IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel)
    {
        return true;
    }

    public void Log<TState>(Microsoft.Extensions.Logging.LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        if (exception == null)
        {
            global::Log.Error(eventId + ": " + state);
            return;
        }

        if (logLevel == Microsoft.Extensions.Logging.LogLevel.Critical)
            global::Log.Critical(exception);

        if (logLevel == Microsoft.Extensions.Logging.LogLevel.Error)
            global::Log.Error(exception);

        if (logLevel == Microsoft.Extensions.Logging.LogLevel.Warning)
            global::Log.Warning(exception);

        if (logLevel == Microsoft.Extensions.Logging.LogLevel.Information)
            global::Log.Information(exception);

        if (logLevel == Microsoft.Extensions.Logging.LogLevel.Debug)
            global::Log.Debug(exception);

        if (logLevel == Microsoft.Extensions.Logging.LogLevel.Trace)
            global::Log.Trace(exception);
    }
}