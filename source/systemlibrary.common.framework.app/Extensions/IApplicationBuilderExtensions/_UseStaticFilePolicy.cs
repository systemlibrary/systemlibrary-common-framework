using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.FileProviders;

namespace SystemLibrary.Common.Framework.App.Extensions;

partial class IApplicationBuilderExtensions
{
    static void UseStaticFilePolicy(IApplicationBuilder app, IWebHostEnvironment env, FrameworkOptions options)
    {
        var contentRootPath = SystemLibrary.Common.Framework.EnvironmentConfig.ContentRootPath;

        if (env?.WebRootPath.Is() == true)
        {
            var webRootPaths = new[] { env.WebRootPath, "public", "static", "dist", "scripts", "frontend", "js", "css", "icons", "images", "fonts", "assets" };

            foreach (var path in webRootPaths)
            {
                if (path.IsNot()) continue;

                var fullPath = Path.Combine(contentRootPath, path);
                try
                {
                    if (!Directory.Exists(fullPath)) continue;
                }
                catch
                {
                    // Swallow: permission or invalid fullpath
                    continue;
                }

                var requestPath = path == env?.WebRootPath ? "" : $"/{path}";

                var staticFileOption = GetStaticFileOptions(fullPath, requestPath, options);

                app.UseStaticFiles(staticFileOption);
            }
        }
        else
        {
            var staticFileOption = GetStaticFileOptions(contentRootPath, "", options);
            app.UseStaticFiles(staticFileOption);
        }
    }

    static StaticFileOptions GetStaticFileOptions(string fullPath, string requestPath, FrameworkOptions options)
    {
        return new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(fullPath),
            RequestPath = requestPath,
            RedirectToAppendTrailingSlash = false,
            ServeUnknownFileTypes = true,
            HttpsCompression = HttpsCompressionMode.Compress,
            OnPrepareResponse = ctx =>
            {
                if (!ctx.Context.Response.Headers.ContainsKey("Cache-Control"))
                    ctx.Context.Response.Headers.Append("Cache-Control", $"public, max-age={options.StaticFilesClientCacheSeconds}");
            }
        };
    }
}