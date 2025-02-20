using System.Diagnostics;
using System.Net;

namespace SystemLibrary.Common.Framework.Tests;

partial class BaseTest
{
    public static void IsOk(object value, string message = "")
    {
        if (!IsValueInvalid(value)) return;

        var msg = GetAssertionMessage(value, message);

        throw new AssertFailedException(msg);
    }

    public static void IsNotOk(object value, string message = "")
    {
        if (IsValueInvalid(value)) return;

        var msg = GetAssertionMessage(value, message);

        throw new AssertFailedException(msg);
    }

    static bool IsValueInvalid(object value)
    {
        if (value == null) return true;

        if(value is HttpStatusCode statusCode)
        {
            return statusCode != HttpStatusCode.OK && 
                statusCode != HttpStatusCode.Accepted;
        }

        if(value is string s)
        {
            if (s.IsNot()) return true;
            var enumKeys = Enum.GetValues<HttpStatusCode>()
                   .Cast<HttpStatusCode>()
                   .Where(status => (int)status >= 400);

            foreach (var enumKey in enumKeys)
                if (s.Contains(enumKey.ToString(), StringComparison.OrdinalIgnoreCase))
                    return true;

            if (s.ContainsAny(StringComparison.OrdinalIgnoreCase, "Invalid", "Error", "Exception"))
                return true;

            return false;
        }

        return value == null ||
               (value is int i && i <= 0) ||               
               (value is double d && d <= 0) ||
               (value is float f && f <= 0) ||
               (value is bool b && !b) ||
               (value is DateTime dt && (dt == DateTime.MinValue || dt == DateTime.MaxValue)) ||
               (value is DateTimeOffset dto && (dto == DateTimeOffset.MinValue || dto == DateTimeOffset.MaxValue));
    }

    static string GetAssertionMessage(object value, string message)
    {
        var ownerType = value?.GetType().Name ?? "Null";
        var stackTrace = new StackTrace();
        var callingMethod = stackTrace.GetFrame(2)?.GetMethod()?.Name ?? "UnknownMethod";

        return message.Is()
           ? $"{message}: {ownerType} got -> {value?.ToString() ?? "null"} at {callingMethod}"
           : $"{ownerType} got -> {value?.ToString() ?? "null"} at {callingMethod}";
    }
}
