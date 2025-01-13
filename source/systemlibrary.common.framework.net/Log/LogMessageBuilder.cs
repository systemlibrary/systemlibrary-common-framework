using System.Collections;
using System.Runtime.CompilerServices;
using System.Text;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;

using SystemLibrary.Common.Framework;

partial class Log
{
    static class LogMessageBuilder
    {
        static bool IsLocal;
        static LogConfig LogOptions;
        static bool MessageFormatJson;

        static LogMessageBuilder()
        {
            IsLocal = EnvironmentConfig.IsLocal == true;
            LogOptions = AppSettings.Current?.SystemLibraryCommonFramework?.Log;
            MessageFormatJson = LogOptions.Format?.Trim()?.ToLower() == "json";
            // Note: in theory it is text|json, but currently any value not equal json ends in "text" format
        }

        internal static string Get(object[] objects, LogLevel level)
        {
            var message = new StringBuilder();
            if (MessageFormatJson)
                message.Append("{\n");

            object firstObj = null;
            bool isLevelNot99999 = (int)level != 99999;
            try
            {
                if (isLevelNot99999)
                    AppendMessageFormat("level", level, message);

                int index = 0;
                foreach (var obj in objects)
                {
                    firstObj ??= obj;

                    if (obj is string json && json.IsJson())
                        AppendMessageFormat("message" + (index > 0 ? index : ""), json, message);
                    else
                        AppendMessage(obj, message, MessageFormatJson ? 1 : 1, index);

                    index++;
                }

                if (isLevelNot99999 && !IsLocal && level == LogLevel.Error && (firstObj as Exception) == null)
                    AppendStackTrace(message);

                var contextAccessor = ServiceProviderInstance.Current.GetService<IHttpContextAccessor>();

                var context = contextAccessor?.HttpContext;

                if (isLevelNot99999 && !IsLocal && context != null)
                {
                    if (LogOptions.AppendPath)
                        AppendRequestPath(message, context.Request);

                    if (LogOptions != null)
                    {
                        if (LogOptions.AppendLoggedInState)
                            AppendLoggedInState(message, context);

                        if (level == LogLevel.Error)
                        {
                            if (LogOptions.AppendBrowser)
                                AppendBrowser(message, context.Request);

                            if (LogOptions.AppendIp)
                                AppendUserIp(message, context);
                        }
                    }

                    if (LogOptions.AppendCorrelationId)
                        AppendCorrelationId(message, context);
                }
            }
            catch (Exception ex)
            {
                AppendMessageFormat("logMessageError", ex.Message + " when logging: " + firstObj, message);
            }

            if (MessageFormatJson)
                message.Append("}\n");

            return message.ToString();
        }

        static string[] CorrelationIdKeys = new[] { "CorrelationId", "correlationId", "correlationid", "CorrelationID", "corrId" };

        static void AppendCorrelationId(StringBuilder message, HttpContext httpContext)
        {
            try
            {

                if (httpContext == null) return;

                var name = "CorrelationId";
                object id = null;

                if (httpContext.Items == null)
                {
                    httpContext.Items = new Dictionary<object, object>();
                }
                else
                {
                    foreach (var key in CorrelationIdKeys)
                    {
                        if (httpContext.Items.TryGetValue(key, out id))
                        {
                            name = key;
                            break;
                        }
                    }
                }

                if (id == null)
                {
                    id = Guid.NewGuid().ToString();
                    httpContext.Items.TryAdd(name, id);
                }

                AppendMessageFormat(name, id, message);
            }
            catch
            {
                AppendMessageFormat("correlationId", null, message);
            }
        }

        static void AppendUserIp(StringBuilder message, HttpContext httpContext)
        {
            try
            {
                var connection = httpContext?.Connection;

                if (connection == null) return;

                var remoteIpAddress = connection?.RemoteIpAddress;

                var userIp = remoteIpAddress?.ToString();

                var wasLocal = false;

                if (userIp.IsNot() || userIp == "::1" || userIp.StartsWith("10.") || userIp.StartsWith("127.0"))
                {
                    userIp = httpContext.Request?.Headers["X-Forwarded-For"].FirstOrDefault();
                    if (userIp.IsNot() || userIp == "::1" || userIp.StartsWith("10.") || userIp.StartsWith("127.0"))
                    {
                        userIp = httpContext.Request?.Headers["REMOTE_ADDR"].FirstOrDefault();
                    }
                    wasLocal = true;
                }

                if (userIp?.Contains(",") == true)
                {
                    userIp = userIp.Split(',')[0];
                }

                if (userIp.IsNot())
                {
                    if (wasLocal)
                        AppendMessageFormat("ip", "local", message);
                    else
                        AppendMessageFormat("ip", "empty", message);
                }

                else if (userIp == "::1" || userIp.StartsWith("10.") || userIp.StartsWith("127.0"))
                    AppendMessageFormat("ip", "local", message);

                else
                    AppendMessageFormat("ip", userIp, message);
            }
            catch
            {
                AppendMessageFormat("ip", "unknown", message);
            }
        }

        static void AppendLoggedInState(StringBuilder message, HttpContext httpContext)
        {
            var isAuthenticated = httpContext?.User?.Identity?.IsAuthenticated == true;

            AppendMessageFormat("authenticated", isAuthenticated, message);
        }

        static void AppendMessage(object obj, StringBuilder message, int level, int index = -1)
        {
            if (level > 2) return;

            if (obj == null)
                AppendMessageFormat("object", null, message, level);

            else if (obj is Exception ex)
            {
                var exmsg = new StringBuilder("", 512);
                if (ex is AggregateException agg)
                {
                    exmsg.Append(agg.Flatten().ToString());
                }
                else
                {
                    exmsg.Append(ex.Message);
                }

                var innerEx = ex.InnerException;
                var c = 0;
                while (innerEx?.Message != null)
                {
                    exmsg.Append("\n" + innerEx.Message);

                    innerEx = innerEx.InnerException;

                    c++;
                    if (c > 5) break;
                }

                AppendMessageFormat("exception", exmsg + "\n" + ex.StackTrace, message, level);
            }

            else if (obj is ITuple ituple)
                AppendMessageFormat("tuple", ituple[0] + ", " + (ituple?.Length > 1 ? ituple[1] + "" : ""), message, level);

            else if (obj is string txt)
                AppendMessageFormat("message" + (index > 0 ? index : ""), txt, message, level);

            else if (obj is IEnumerable enumerable)
                AppendClass(message, enumerable);
            else if (IsLoggableClass(obj))
                AppendClass(message, obj);
            else
                AppendMessageFormat("message" + (index > 0 ? index : ""), obj.ToString(), message);
        }

        static void AppendMessageFormat(string name, object obj, StringBuilder message, int level = -1)
        {
            if (level > 2) return;

            if (MessageFormatJson)
                AppendJsonFormat(name, obj, message);
            else
                AppendPlainFormat(name, obj, message);
        }

        static void AppendJsonFormat(string name, object obj, StringBuilder message)
        {
            if (obj == null)
                message.Append("\t\"" + name + "\": null,\n");
            else
                message.Append("\t\"" + name + "\": \"" + obj + "\",\n");
        }

        static void AppendPlainFormat(string name, object obj, StringBuilder message)
        {
            if (name == "level")
            {
                message.Append(obj + ": ");
            }
            else if (name == "message")
            {
                if (obj == null)
                    message.Append("(null)\n");
                else
                    message.Append(obj + "\n");
            }
            else if (obj == null)
                message.Append(name + ": null\n");
            else if (name == "stacktrace")
                message.Append(name + ": " + obj + "\n");
            else if (name == "exception")
                message.Append(obj + "\n");
            else
                message.Append(name + ": " + obj + "\n");
        }

        static void AppendClass(StringBuilder message, object obj)
        {
            var objAsString = new StringBuilder();

            Dump.Build(objAsString, obj, 0, 3);

            AppendMessageFormat("object", objAsString.ToString(), message);
        }

        static bool IsLoggableClass(object o)
        {
            var type = o.GetType();

            return type.IsClass &&
                !type.IsEnum &&
                !type.IsArray &&
                type != SystemType.StringType &&
                type != SystemType.ExceptionType &&
                type != typeof(StringBuilder) &&
                type != typeof(Nullable) &&
                type != typeof(Nullable<>) &&
                type != typeof(NullReferenceException) &&
                type != typeof(RuntimeWrappedException);
        }

        static void AppendBrowser(StringBuilder message, HttpRequest request)
        {
            string userAgent = null;

            if (request?.Headers?.ContainsKey(HeaderNames.UserAgent) == true)
                userAgent = request.Headers[HeaderNames.UserAgent];

            AppendMessageFormat("agent", userAgent, message);
        }

        static void AppendStackTrace(StringBuilder message)
        {
            try
            {
                var stackTraceBuilder = new StringBuilder("");
                var stackTrace = Environment.StackTrace?.ToString();
                if (stackTrace != null)
                {
                    var traces = stackTrace.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
                    var end = Math.Min(traces.Length, 9);
                    for (int i = 0; i < end; i++)
                    {
                        if (traces[i].StartsWithAny(
                            "   at System.RuntimeMethodHandle",
                            "   at System.Reflection.RuntimeMethodInfo",
                            "   at lambda_method"))
                        {
                            break;
                        }

                        if (traces[i].StartsWithAny(
                            "   at Log.Write(Object obj, LogLevel level)",
                            "   at Log.LogMessageBuilder",
                            "   at System.Environment.get_",
                                "   at Log.AppendStackTrace"))
                        {
                            continue;
                        }

                        if (i < Math.Min(traces.Length, 9) - 1)
                        {
                            if (MessageFormatJson)
                                stackTraceBuilder.Append("\t" + traces[i].TrimStart() + "\n");
                            else
                            {
                                stackTraceBuilder.Append("\t" + traces[i].TrimStart() + "\n");
                            }
                        }

                    }
                }
                AppendMessageFormat("stacktrace", stackTraceBuilder.ToString(), message);
            }
            catch
            {
                AppendMessageFormat("stacktrace", "unknown", message);
            }
        }

        static void AppendRequestPath(StringBuilder message, HttpRequest request)
        {
            if (request?.Path != null)
                AppendMessageFormat("path", request.Path.Value + request.QueryString.Value, message);
        }
    }
}
