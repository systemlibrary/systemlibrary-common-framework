using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;

using SystemLibrary.Common.Framework;

partial class Log
{
    static class LogMessageBuilder
    {
        static bool IsLocal;
        static LogConfig LogConfig;
        static LogFormat Format;
        static JsonStringEnumConverter JsonStringEnumConverter = new();

        static LogMessageBuilder()
        {
            IsLocal = EnvironmentConfig.IsLocal;
            LogConfig = AppSettings.Current.SystemLibraryCommonFramework.Log;
            Format = LogConfig.Format;
        }

        internal static string Get(object[] objects, LogLevel level)
        {
            var message = new StringBuilder(256);

            if(level != LogLevel.Unset && (int)level < 9999)
                message.Append(level.ToString().ToUpper() + ": ");

            foreach (var obj in objects)
            {
                message.Append(Build(obj));

                message.Append("\n");
            }

            if (level != LogLevel.Critical &&
                level != LogLevel.Error &&
                level != LogLevel.Debug &&
                level != LogLevel.Warning)
                return message.ToString();

            if (IsLocal)
                return message.ToString();

            var httpContext = ServiceProviderInstance.Current.GetService<IHttpContextAccessor>()?.HttpContext;

            var url = GetUrl(httpContext?.Request);
            var httpMethod = GetHttpMethod(httpContext?.Request);
            var isAuthenticated = GetAuthenticatedState(httpContext);
            var stackTrace = GetStackTrace();
            var IP = GetIP(httpContext);
            var browserName = GetBrowserName(httpContext?.Request);
            var correlationId = GetCorrelationId(httpContext);

            if (Format == LogFormat.Json)
            {
                var anonymous = new
                {
                    message = message.ToString(),
                    url = httpMethod.Is() ? "(" + httpMethod + ") " + url : url,
                    stackTrace,
                    isAuthenticated,
                    IP,
                    browserName,
                    correlationId
                };

                // Note: cannot be a singleton, it crashes occasionally in high concurrency apps
                var options = new JsonSerializerOptions()
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                    MaxDepth = 1,
                    IncludeFields = true,
                    AllowTrailingCommas = true,
                    IgnoreReadOnlyFields = true,
                    IgnoreReadOnlyProperties = false,
                    WriteIndented = true,
                };

                options.Converters.Add(JsonStringEnumConverter);

                return JsonSerializer.Serialize(anonymous, options);
            }
            else
            {
                if (url != null)
                    message.Append("url: " + (httpMethod.Is() ? "(" + httpMethod + ") " + url + "\n" : url + "\n"));

                if (stackTrace != null)
                    message.Append("stackTrace: " + stackTrace + "\n");

                if (isAuthenticated != null)
                    message.Append("isAuthenticated: " + isAuthenticated + "\n");

                if (IP != null)
                    message.Append("IP: " + IP + "\n");

                if (browserName != null)
                    message.Append("browser: " + browserName + "\n");

                if (correlationId != null)
                    message.Append("correlationId: " + correlationId + "\n");

                return message.ToString();
            }
        }

        static string[] CorrelationIdKeys = ["CorrelationId", "correlationId", "CorrelationID"];

        static string GetStackTrace()
        {
            if (!LogConfig.AddStacktrace) return null;

            try
            {
                var stackTrace = Environment.StackTrace?.ToString();
                if (stackTrace != null)
                {
                    var stackTraceBuilder = new StringBuilder("");

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
                            stackTraceBuilder.Append("\t" + traces[i].TrimStart() + "\n");
                    }
                    return stackTraceBuilder.ToString().TrimStart('\t').TrimEnd("\n");
                }
                return "";
            }
            catch
            {
                return "unknown";
            }
        }

        static string GetUrl(HttpRequest request)
        {
            if (!LogConfig.AddUrl) return null;

            if (request?.Path != null)
                return request.Path.Value + request.QueryString.Value;

            return "";
        }

        static string GetHttpMethod(HttpRequest request)
        {
            if (!LogConfig.AddHttpMethod) return null;

            if (request?.Method != null)
                return request.Method;
            return "";
        }

        static string GetAuthenticatedState(HttpContext httpContext)
        {
            if (!LogConfig.AddAuthenticatedState) return null;

            if (httpContext?.User?.Identity?.IsAuthenticated == true)
                return "true";

            return "false";
        }

        static string GetIP(HttpContext httpContext)
        {
            if (!LogConfig.AddIP) return null;

            try
            {
                var connection = httpContext?.Connection;

                if (connection == null) return null;

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
                        return "local";
                    return
                        "empty";
                }

                else if (userIp == "::1" || userIp.StartsWith("10.") || userIp.StartsWith("127.0"))
                    return "local";

                return userIp;
            }
            catch
            {
                return "unknown";
            }
        }

        static string GetBrowserName(HttpRequest request)
        {
            if (!LogConfig.AddBrowserName) return null;

            var userAgent = request?.Headers[HeaderNames.UserAgent].FirstOrDefault();

            if (userAgent == null || userAgent.Length < 5) return "";

            if (userAgent.Contains("Edg", StringComparison.OrdinalIgnoreCase)) return "Edge";
            if (userAgent.Contains("OPR", StringComparison.OrdinalIgnoreCase) || userAgent.Contains("Opera", StringComparison.OrdinalIgnoreCase)) return "Opera";
            if (userAgent.Contains("Brave", StringComparison.OrdinalIgnoreCase)) return "Brave";
            if (userAgent.Contains("Chrome", StringComparison.OrdinalIgnoreCase)) return "Chrome";
            if (userAgent.Contains("Safari", StringComparison.OrdinalIgnoreCase)) return "Safari";
            if (userAgent.Contains("Firefox", StringComparison.OrdinalIgnoreCase)) return "Firefox";

            return "unknown";
        }

        static string GetCorrelationId(HttpContext httpContext)
        {
            if (!LogConfig.AddCorrelationId) return null;

            var items = httpContext?.Items;

            if (items == null) return "";

            try
            {
                var name = "CorrelationId";
                object id = null;
                foreach (var key in CorrelationIdKeys)
                {
                    if (httpContext.Items.TryGetValue(key, out id))
                    {
                        name = key;
                        break;
                    }
                }

                if (id == null)
                {
                    id = Guid.NewGuid().ToString();

                    httpContext.Items.TryAdd(name, id);
                }

                return (string)id;
            }
            catch
            {
                return "unknown";
            }
        }
    }
}
