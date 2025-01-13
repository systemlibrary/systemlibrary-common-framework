﻿using Microsoft.Extensions.Logging;

namespace SystemLibrary.Common.Framework;

internal class InternalLog : ILogger
{
    public IDisposable BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        if (exception == null)
        {
            global::Log.Error(logLevel + " " + eventId + ": " + state);
            return;
        }

        if (logLevel == LogLevel.Critical)
            global::Log.Error(exception);

        if (logLevel == LogLevel.Error)
            global::Log.Error(exception);

        if (logLevel == LogLevel.Warning)
            global::Log.Warning(exception);

        if (logLevel == LogLevel.Information)
            global::Log.Information(exception);

        if (logLevel == LogLevel.Debug)
            global::Log.Debug(exception);

        if (logLevel == LogLevel.Trace)
            global::Log.Trace(exception);
    }
}