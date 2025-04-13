using System.Runtime.CompilerServices;

using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.Extensions.DependencyInjection;

using SystemLibrary.Common.Framework;

/// <summary>
/// Log class, takes any object, creating a meaningful message based on the current request data, and sending the message to your LogWriter.
/// <para>You implement LogWriter : ILogWriter, which controls where the message is stored.</para>
/// Register LogWriter as a service in your application.
/// </summary>
/// <remarks>
/// Log.Error() creates a log message, which in turn calls your LogWriter with the message.
/// <para>- You can then add additional data to the message before storing it, as needed.</para>
/// Log exists in the global namespace.
/// <para>If ILogWriter is not registered, this will use Log.Dump. Note: Log.Dump should not be used in production for performance and disc reasons.</para>
/// </remarks>
/// <example>
/// Configure log options in appsettings.json
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

    static bool LogIsOff
    {
        get
        {
            return (LogLevel)MinLogLevel == LogLevel.None;
        }
    }

    static object MinLogLevelLock = new object();

    static bool MinLogLevelIsSet = false;
    static int _MinLogLevel;

    static int MinLogLevel
    {
        get
        {
            if (!MinLogLevelIsSet)
            {
                lock (MinLogLevelLock)
                {
                    if (MinLogLevelIsSet) return _MinLogLevel;

                    var temp = FrameworkConfigInstance.Current.Log.Level;

                    // Not specified for the package 'systemLibraryCommonFramework', let's check the global logging level
                    if (temp == LogLevel.Unset)
                    {
                        // If 'none' is the default logging level, no logs should occur at all, except Log.Write always bypassed, same does Log.Dump (but this writes to physical drive)
                        var defaultLogLevel = AppSettings.Current.Logging.LogLevel.Default.ToLower();

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

                    MinLogLevelIsSet = true;
                }
            }
            return _MinLogLevel;
        }
    }

    static Log()
    {
        BlacklistClassTypes.Add(typeof(RuntimeTypeHandle).Name);
        BlacklistClassTypes.Add(typeof(ModelBindingMessageProvider).Name);
        BlacklistClassTypes.Add(typeof(RuntimeWrappedException).Name);

        BlacklistClassTypes.Add("RuntimeAssembly");
        BlacklistClassTypes.Add("RuntimeModule");
        BlacklistClassTypes.Add("RuntimeMethodHandle");
        BlacklistClassTypes.Add("ControllerContext");

        BlacklistMemberNames.Add("Constructor");
        BlacklistMemberNames.Add("ReturnTypeCustomAttributes");
        BlacklistMemberNames.Add("ReturnParameter");
        BlacklistMemberNames.Add("MethodImplementationFlags");
        BlacklistMemberNames.Add("CallingConvention");

        //BlacklistMemberNames.Add("ConstructorArguments");

        //BlacklistClassTypes.Add(typeof(RuntimeTypeHandle).Name);
        //BlacklistClassTypes.Add(typeof(ModelBindingMessageProvider).Name);
        //BlacklistClassTypes.Add(typeof(SafeWaitHandle).Name);
        //BlacklistClassTypes.Add(typeof(RuntimeWrappedException).Name);
        //BlacklistClassTypes.Add(typeof(char).Name);
        //BlacklistClassTypes.Add("RuntimeMethodHandle");
        //BlacklistClassTypes.Add("RuntimeCustomAttributeData");
        //BlacklistClassTypes.Add("RuntimeModule");
        //BlacklistMemberNames.Add("RuntimeConstructorInfo");
        //BlacklistMemberNames.Add("ReturnTypeCustomAttributes");
        //BlacklistMemberNames.Add("Constructor");

        FullFilePath = FrameworkConfigInstance.Current.Log.GetFullFilePath();

        if (FullFilePath.Is())
        {
            var folder = new FileInfo(FullFilePath).DirectoryName + "\\";

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
        }
    }

    /// <summary>
    /// Write an critical message
    /// </summary>
    /// <param name="obj">Object can be of any type, a string, list, dictionary, etc...</param>
    /// <example>
    /// Usage:
    /// <code>
    /// Log.Critical("hello world");
    /// //This creates a log message prefixed with Timestamp then Level then your input and sends it to Log.Dump or your ILogWriter
    /// </code>
    /// </example>
    public static void Critical(params object[] obj)
    {
        Write(obj, LogLevel.Critical);
    }

    /// <summary>
    /// Write an error message
    /// </summary>
    /// <param name="obj">Object can be of any type, a string, list, dictionary, etc...</param>
    /// <example>
    /// Usage:
    /// <code>
    /// Log.Error("hello world");
    /// //This creates a log message prefixed with Timestamp then Level then your input and sends it to Log.Dump or your ILogWriter
    /// </code>
    /// </example>
    public static void Error(params object[] obj)
    {
        Write(obj, LogLevel.Error);
    }

    /// <summary>
    /// Write an warning message
    /// </summary>
    /// <param name="obj">Object can be of any type, a string, list, dictionary, etc...</param>
    /// <example>
    /// Usage:
    /// <code>
    /// Log.Warning("hello world");
    /// //This creates a log message prefixed with Timestamp then Level then your input and sends it to Log.Dump or your ILogWriter
    /// </code>
    /// </example>
    public static void Warning(params object[] obj)
    {
        Write(obj, LogLevel.Warning);
    }

    /// <summary>
    /// Write an debug message
    /// </summary>
    /// <param name="obj">Object can be of any type, a string, list, dictionary, etc...</param>
    /// <example>
    /// Usage:
    /// <code>
    /// Log.Debug("hello world");
    /// //This creates a log message prefixed with Timestamp then Level then your input and sends it to Log.Dump or your ILogWriter
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
    /// //This creates a log message prefixed with Timestamp then Level then your input and sends it to Log.Dump or your ILogWriter
    /// </code>
    /// </example>
    public static void Information(params object[] obj)
    {
        Write(obj, LogLevel.Information);
    }

    /// <summary>
    /// Write an trace message
    /// </summary>
    /// <param name="obj">Object can be of any type, a string, list, dictionary, etc...</param>
    /// <example>
    /// Usage:
    /// <code>
    /// Log.Trace("hello world");
    /// //This creates a log message prefixed with Timestamp then Level then your input and sends it to Log.Dump or your ILogWriter
    /// </code>
    /// </example>
    public static void Trace(params object[] obj)
    {
        Write(obj, LogLevel.Trace);
    }

    /// <summary>
    /// Always writes the message to your LogWriter, regardless of the minimum log level or whether logging is set to 'None'. You decide if the message should be sent further.
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
            if (LogIsOff)
            {
                if (!WarningDumped)
                {
                    WarningDumped = true;
                    Log.Dump("[SystemLibrary.Common.Framework] Logging is unset for the framework and the default logging is set to 'none', will not log anything from here on.");
                }
                return;
            }

            if ((int)level < MinLogLevel) return;
        }

        // TODO: Optimize by 'fire and forget' the whole log message builder and dumping/logging
        var message = LogMessageBuilder.Get(obj, level);

        var prefix = level.ToString().ToUpper() + ": ";
        if (message.StartsWith(prefix + prefix + prefix))
        {
            global::SystemLibrary.Common.Framework.Debug.Write("Infinite loop - you're using the LogWriter to call the Log and LogBuilder, your LogWriter should write to somewhere the message, not pass it into the SystemLibrary.Common.Framework.Log...");
            throw new Exception("Infinite loop - your ILogWriter implementation calls Log within SystemLibrary.Common.Framework. You have the message already, you must send it to somewhere for storage, your local disc, sentry, cloudwatch, firebase, azure app insight, splunk, log4net, serilog or similar...");
        }

        if (LogWriter == null)
        {
            if (!WarningDumped)
            {
                WarningDumped = true;
                Log.Dump("SystemLibrary.Common.Framework.App.ILogWriter is not registered as a service, will Log.Dump message...");
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


