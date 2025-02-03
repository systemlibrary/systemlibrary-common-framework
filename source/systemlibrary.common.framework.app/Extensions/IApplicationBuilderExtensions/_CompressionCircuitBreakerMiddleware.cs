using Microsoft.AspNetCore.Http;

using SystemLibrary.Common.Framework.App.Extensions;

internal static class Compress
{
    internal static bool IsEligibleForCompression(HttpContext context, FrameworkAppOptions options)
    {
        if (context?.Request == null)
            return false;

        if (context.WebSockets?.IsWebSocketRequest == true)
            return false;

        if (context.Response?.Headers?.ContainsKey("Content-Encoding") == true)
            return false;

        if (context.Request.Headers == null) return true;

        var acceptEncoding = context.Request.Headers["Accept-Encoding"].ToString();

        return acceptEncoding.IsNot() || acceptEncoding.Contains("br") || acceptEncoding.Contains("gzip");
    }
}
