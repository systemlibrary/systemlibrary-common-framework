using Microsoft.AspNetCore.Http;

namespace SystemLibrary.Common.Framework.Extensions;

internal static class BlacklistedRequestMiddleware
{
    static string[] BlockedExtensions = [
        ".exe", ".dll", ".iso", ".msi", ".ps1", ".cmd", ".sh", ".bash",
        ".vbs", ".dmg", ".config", ".env", ".ini", ".key", ".pem",
        ".cshtml", ".cs"
    ];

    static int BlockedExtensionsLength = BlockedExtensions.Length;

    public static async Task Use(HttpContext context, RequestDelegate next)
    {
        var requestPath = context?.Request?.Path.Value;

        if (requestPath == null)
        {
            await next(context);
            return;
        }

        var l = requestPath.Length;

        if (l <= 5)
        {
            await next(context);
            return;
        }

        if (requestPath.StartsWith("/app_data/", StringComparison.OrdinalIgnoreCase) ||
            requestPath.StartsWith("/properties/", StringComparison.OrdinalIgnoreCase) ||
            requestPath.StartsWith("/bin/", StringComparison.OrdinalIgnoreCase))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return;
        }

        if (requestPath[l - 1] == '/')
        {
            await next(context);
            return;
        }

        var extensionPosition = requestPath.LastIndexOf('.', l - 1, 7);

        if (extensionPosition == -1)
        {
            await next(context);
            return;
        }

        var extensionLength = l - extensionPosition;

        if (extensionLength < 3)
        {
            await next(context);
            return;
        }

        for (int i = 0; i < BlockedExtensionsLength; i++)
        {
            if (requestPath.EndsWith(BlockedExtensions[i], StringComparison.OrdinalIgnoreCase))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return;
            }
        }

        await next(context);
    }
}