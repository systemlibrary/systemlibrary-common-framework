using System.Runtime.CompilerServices;

using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32.SafeHandles;

using SystemLibrary.Common.Framework;

/// <summary>
/// Log class, responsible for taking any object and create a decent message, based on current request data, and send the whole message as a string to your LogWriter
/// <para>You implement LogWriter : ILogWriter, which controls where you store the message</para>
/// You register LogWriter as a service in your application
/// </summary>
/// <remarks>
/// Log.Error() creates a log message which again calls your LogWriter with the message
/// <para>- You can then add additional data to the message before storing, up to you</para>
/// Log exists in the global namespace
/// <para>If ILogWriter is not registered, this will use Dump.Write. Dump.Write shall never be used in production</para>
/// </remarks>
/// <example>
/// Configure log options in appSettings.json
/// <code>
/// {
///     "systemLibraryCommonFramework": {
///         "log": {
///             "level": "Information" // Trace, Information, Debug, Warning, Error, None
///             "appendPath": true,
///         	"appendLoggedInState": true,
///         	"appendCorrelationId": true,
///         	"appendIp": false,
///         	"appendBrowser": false,
///         	"format": null // "json" or null, null is plain text
///         }
///     }
/// }
/// </code>
/// </example>
public static partial class Log
{
    static readonly HashSet<string> BlacklistMemberNames = new();
    static readonly HashSet<string> BlacklistClassTypes = new();

    static string FullFilePath;

    static readonly ILogWriter LogWriter = ServiceProviderInstance.Current.GetService<ILogWriter>();

    static bool WarningDumped = false;

    static readonly bool LogIsOff = (LogLevel)MinLogLevel == LogLevel.None;

    static int? _MinLogLevel;
    static int MinLogLevel
    {
        get
        {
            if (_MinLogLevel == null)
            {
                var temp = FrameworkConfig.Current?.Log?.Level;

                // Not specified for the package 'systemLibraryCommonFramework', let's check the global logging level
                if (temp == null || temp == LogLevel.Unset)
                {
                    // If 'none' is the default logging level, no logs should occur at all, except Log.Write always bypassed, same does Log.Dump (but this writes to physical drive)
                    var defaultLogLevel = AppSettings.Current.Logging?.LogLevel?.Default?.ToLower();

                    if (defaultLogLevel.Is())
                    {
                        if (defaultLogLevel == "none")
                            temp = LogLevel.None;
                        else
                            temp = defaultLogLevel.ToEnum<LogLevel>();
                    }
                    else
                        temp = LogLevel.Debug; // Debug, Error and Critical are logged by default
                }
                _MinLogLevel = (int)temp;
            }
            return _MinLogLevel.Value;
        }
    }

    static Log()
    {
        BlacklistClassTypes.Add(typeof(Exception).Name);
        BlacklistClassTypes.Add(typeof(NullReferenceException).Name);
        BlacklistClassTypes.Add(typeof(RuntimeTypeHandle).Name);
        BlacklistClassTypes.Add(typeof(ModelBindingMessageProvider).Name);
        BlacklistClassTypes.Add(typeof(SafeWaitHandle).Name);
        BlacklistClassTypes.Add(typeof(RuntimeWrappedException).Name);
        BlacklistClassTypes.Add(typeof(char).Name);
        BlacklistClassTypes.Add("RuntimeType");
        BlacklistClassTypes.Add("RuntimeMethodInfo");
        BlacklistClassTypes.Add("RuntimeAssembly");
        BlacklistClassTypes.Add("Constructor");

        FullFilePath = FrameworkConfig.Current.Log.GetFullFilePath();

        if (FullFilePath.Is())
        {
            var folder = new FileInfo(FullFilePath).DirectoryName + "\\";

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
        }
    }

    /// <summary>
    /// Write an error message
    /// </summary>
    /// <param name="obj">Object can be of any type, a string, list, dictionary, etc...</param>
    /// <example>
    /// Usage:
    /// <code>
    /// Log.Error("hello world");
    /// //This creates a log message with prefix 'Error', timestamp, stacktrace and your input text "hello world" and sends it to your LogWriter
    /// </code>
    /// </example>
    public static void Error(params object[] obj)
    {
        Write(obj, LogLevel.Error);
    }

    /// <summary>
    /// Write a warning message
    /// </summary>
    /// <param name="obj">Object can be of any type, a string, list, dictionary, etc...</param>
    /// <example>
    /// Usage:
    /// <code>
    /// Log.Warning("hello world");
    /// //This creates a log message with prefix 'Warning', timestamp and your input text "hello world" and sends it to your LogWriter
    /// </code>
    /// </example>
    public static void Warning(params object[] obj)
    {
        Write(obj, LogLevel.Warning);
    }

    /// <summary>
    /// Write a debug message
    /// </summary>
    /// <param name="obj">Object can be of any type, a string, list, dictionary, etc...</param>
    /// <example>
    /// Usage:
    /// <code>
    /// Log.Debug("hello world");
    /// //This creates a log message with prefix 'Debug', timestamp and your input text "hello world" and sends it to your LogWriter
    /// </code>
    /// </example>
    public static void Debug(params object[] obj)
    {
        Write(obj, LogLevel.Debug);
    }

    /// <summary>
    /// Write an information message
    /// </summary>
    /// <param name="obj">Object can be of any type, a string, list, dictionary, etc...</param>
    /// <example>
    /// Usage:
    /// <code>
    /// Log.Information("hello world");
    /// //This creates a log message with prefix 'Info', timestamp and your input text "hello world" and sends it to your LogWriter
    /// </code>
    /// </example>
    public static void Information(params object[] obj)
    {
        Write(obj, LogLevel.Information);
    }

    /// <summary>
    /// Write a trace message
    /// </summary>
    /// <param name="obj">Object can be of any type, a string, list, dictionary, etc...</param>
    /// <example>
    /// Usage:
    /// <code>
    /// Log.Trace("hello world");
    /// //This creates a log message with prefix 'Trace', timestamp and your input text "hello world" and sends it to your LogWriter
    /// </code>
    /// </example>
    public static void Trace(params object[] obj)
    {
        Write(obj, LogLevel.Trace);
    }

    /// <summary>
    /// Always writing the message to your LogWriter
    /// <para>This ignores the log level set in appSettings, so it always writes</para>
    /// </summary>
    /// <param name="obj">Object can be of any type, a string, list, dictionary, etc...</param>
    /// <example>
    /// <code>
    /// var list = new List&lt;User&gt;
    /// list.Add(new User { firstName = "hello", LastName = "World" });
    /// 
    /// Log.Write(list);
    /// //This creates a log message with your input and sends it as a string to your LogWriter
    /// //Note: Log.Write() can never be disabled/turned off, remove the calls if you dont want them in production
    /// </code>
    /// </example>
    public static void Write(params object[] obj)
    {
        Write(obj, (LogLevel)99999);
    }

    static void Write(object[] obj, LogLevel level)
    {
        if ((int)level != 99999)
        {
            if (LogIsOff) return;

            if ((int)level < MinLogLevel) return;
        }

        // TODO: Optimize by 'fire and forget' the whole log message builder and dumping/logging
        var message = LogMessageBuilder.Get(obj, level);

        if (LogWriter == null)
        {
            if (!WarningDumped)
            {
                WarningDumped = true;
                Log.Dump("SystemLibrary.Common.Framework.App.ILogWriter is not registered as a service, will Log.Dump message");
            }
            Log.Dump(message);
            return;
        }

        switch (level)
        {
            case LogLevel.Critical:
                LogWriter.Critical(message); 
                break;
            case LogLevel.Error:
                LogWriter.Error(message);
                break;
            case LogLevel.Debug:
                LogWriter.Debug(message);
                break;
            case LogLevel.Warning:
                LogWriter.Warning(message);
                break;
            case LogLevel.Information:
                LogWriter.Information(message);
                break;
            case LogLevel.Trace:
                LogWriter.Trace(message);
                break;
            default:
                LogWriter.Write(message);
                break;
        }
    }
}


