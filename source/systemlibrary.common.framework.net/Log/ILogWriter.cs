﻿namespace SystemLibrary.Common.Framework;

/// <summary>
/// ILogWriter is responsible for storing log messages
/// <para>- Create a new class that implements ILogWriter</para>
/// - Register your class as singleton implementation of the ILogWriter
/// </summary>
/// <example>
/// Program.cs/Startup.cs:
/// <code>
/// public void ConfigureServices(IServiceCollection services)
/// {
///     services.AddSingleton(typeof(YourLogWriter), typeof(ILogWriter));
/// }
/// </code>
/// 
/// Your LogWriter implementation:
/// <code>
/// public class YourLogWriter : ILogWriter
/// { 
///     public void Error(string message) 
///     {
///         //Write message to slack? CloudWatch? Sentry.io? Local disc? Its up to you
///     }
///     ...
/// }
/// </code>
/// 
/// Usage
/// <code>
/// // The hierarchy is as follows:
/// Log.Write("hello world");
/// Log.Critical("hello world");
/// Log.Error("hello world");
/// Log.Debug("hello world");
/// Log.Warning("hello world");
/// Log.Info("hello world");
/// Log.Trace("hello world");
/// </code>
/// All calls to method in 'Log' will create a message, append various data, and pass it to your log writer
/// </example>
public interface ILogWriter
{
    /// <summary>
    /// Implement the writing of critical messages
    /// </summary>
    void Critical(string message);

    /// <summary>
    /// Implement the writing of error messages
    /// </summary>
    void Error(string message);

    /// <summary>
    /// Implement the writing of debug messages
    /// </summary>
    void Debug(string message);

    /// <summary>
    /// Implement the writing of warning messages
    /// </summary>
    void Warning(string message);

    /// <summary>
    /// Implement the writing of information messages
    /// </summary>
    void Information(string message);

    /// <summary>
    /// Implement the writing of trace messages
    /// </summary>
    void Trace(string message);

    /// <summary>
    /// Implement the writing of warning messages
    /// <para>
    /// Note: Write() will always be invoked, even if you disable logging in appSettings
    /// </para>
    /// </summary>
    void Write(string message);
}
