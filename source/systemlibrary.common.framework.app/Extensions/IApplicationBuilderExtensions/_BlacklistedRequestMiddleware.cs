using Microsoft.AspNetCore.Http;

namespace SystemLibrary.Common.Framework.Extensions;

internal static class BlacklistedRequestMiddleware
{
    static string[] BlockedExtensions = [
        ".exe", ".dll", ".iso", ".msi", ".ps1", ".cmd", ".sh", ".bash", ".vbs", ".dmg",
        ".config", ".env", ".ini", ".key", ".pem", ".cshtml", ".cs", ".sql", ".mdf", ".tsx", ".jsx",
        ".bat", ".jar", ".php", ".py", ".pl", ".rb", ".go", ".vb", ".vbs", ".hta", ".bak", ".db", ".pfx", ".crt"
    ];

    static int BlockedExtensionsLength = BlockedExtensions.Length;

    public static async Task Use(HttpContext context, RequestDelegate next)
    {
        string text = context?.Request?.Path.Value;
        if (text == null)
        {
            await next(context);
            return;
        }

        int length = text.Length;
        if (length <= 5)
        {
            await next(context);
            return;
        }


        if (text.StartsWith("/app_data/", StringComparison.OrdinalIgnoreCase) ||
            text.StartsWith("/properties/", StringComparison.OrdinalIgnoreCase) ||
            text.StartsWith("/configs/", StringComparison.OrdinalIgnoreCase) ||
            text.StartsWith("/configurations/", StringComparison.OrdinalIgnoreCase) ||
            text.StartsWith("/bin/", StringComparison.OrdinalIgnoreCase) ||
            text.StartsWith("/obj/", StringComparison.OrdinalIgnoreCase))
        {
            context.Response.Headers.TryAdd("Reason", "Access denied by Common Framework");
            context.Response.StatusCode = 403;
            return;
        }

        if (text[length - 1] == '/')
        {
            await next(context);
            return;
        }

        int num = text.LastIndexOf('.', length - 1, 7);
        if (num == -1)
        {
            await next(context);
            return;
        }

        if (length - num < 3)
        {
            await next(context);
            return;
        }

        if(text.Contains("appsettings.", StringComparison.OrdinalIgnoreCase))
        {
            context.Response.Headers.TryAdd("Reason", "Access denied by Common Framework");
            context.Response.StatusCode = 403;
            return;
        }

        for (int i = 0; i < BlockedExtensionsLength; i++)
        {
            if (text.EndsWith(BlockedExtensions[i], StringComparison.OrdinalIgnoreCase))
            {
                context.Response.Headers.TryAdd("Reason", "Access denied by Common Framework");
                context.Response.StatusCode = 403;
                return;
            }
        }

        await next(context);
    }
}