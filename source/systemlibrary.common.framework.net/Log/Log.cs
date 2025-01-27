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
    static HashSet<string> BlacklistMemberNames = new();
    static HashSet<string> BlacklistClassTypes = new();

    static string FullFilePath;

    static ILogWriter _LogWriter;
    static ILogWriter LogWriter
    {
        get
        {
            _LogWriter ??= ServiceProviderInstance.Current.GetService<ILogWriter>();

            return _LogWriter;
        }
    }
    static bool WarningDumped = false;

    static bool? _LogIsOff;
    static bool LogIsOff
    {
        get
        {
            if (_LogIsOff == null)
            {
                // Turned off for this package config
                var temp = FrameworkConfig.Current.Log.Level;

                if (temp == null)
                {
                    // Turned off on a global level, so one can turn off globally, but explicit enable for this package
                    if (AppSettings.Current.Logging.LogLevel.Default?.ToLower() == "none")
                        temp = LogLevel.None;
                }

                _LogIsOff = temp == LogLevel.None;

            }
            return _LogIsOff.Value;
        }
    }

    static int? _MinLogLevel;
    static int MinLogLevel
    {
        get
        {
            if (_MinLogLevel == null)
            {
                // Read log level specific to this package
                var temp = AppSettings.Current?.SystemLibraryCommonFramework?.Log?.Level;

                if (temp == null)
                {
                    // Package log was not specified, check the global default "Logging" if exists
                    var def = AppSettings.Current?.Logging?.LogLevel?.Default;
                    if (def.Is())
                    {
                        temp = def.ToEnum<LogLevel>();
                    }
                    else
                    {
                        // Setting default
                        temp = LogLevel.Information;
                    }
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
                Log.Dump("SystemLibrary.Common.Framework.App.ILogWriter is not yet registered as a service, will Dump.Write message");
            }
            Log.Dump(message);
            return;
        }

        switch (level)
        {
            case LogLevel.Error:
                LogWriter.Error(message);
                break;
            case LogLevel.Warning:
                LogWriter.Warning(message);
                break;
            case LogLevel.Debug:
                LogWriter.Debug(message);
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


