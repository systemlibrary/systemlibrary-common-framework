using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.FileProviders;

using Prometheus;

using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework.App.Extensions;

/// <summary>
/// Extension methods for ApplicationBuilder object
/// </summary>
public static partial class IApplicationBuilderExtensions
{
    /// <summary>
    /// Register common middlewares for a web application
    /// <para>This will register:</para>
    /// - Http to Https redirection middleware, client and server side
    /// <para>- Routing urls to controllers middleware</para>
    /// - /api/ urls to controllers middleware
    /// <para>- Authentication and Authorization attributes' middleware</para>
    /// - Servince static files such as .css, .js, .jpg, etc... middleware
    /// <para>- Forwarded headers middleware</para>
    /// - Razor pages and Mvc middleware
    /// <para>- Secure cookie policy middleware</para>
    /// - Secure cookie policy (http only middleware
    /// <para>- Recompiling razor pages (saving a cshtml file) middleware</para>
    /// - Exception page middleware
    /// </summary>
    /// <remarks>
    /// This should be your first registration of middlewares you have, exception might be your own middleware for logging/tracing requests
    /// </remarks>
    /// <example>
    /// Startup.cs/Program.cs:
    /// <code>
    /// public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    /// {
    ///     var options = new FrameworkAppOptions();
    ///     
    ///     app.UseFrameworkApp(options);
    /// }
    /// </code>
    /// </example>
    public static IApplicationBuilder UseFrameworkApp(this IApplicationBuilder app, IWebHostEnvironment env, FrameworkAppOptions options = null)
    {
        app.ApplicationServices.UseFrameworkServiceProvider();

        options ??= new FrameworkAppOptions();

        if (options.UseDeveloperPage)
            app.UseDeveloperExceptionPage();

        if (options.UseForwardedHeaders)
            app.UseForwardedHeaders();

        if (options.UseHsts)
            app.UseHsts();

        if (options.UseHttpsRedirection)
            app.UseHttpsRedirection();

        if (options.UseStaticFiles)
        {
            var contentRootPath = env?.WebRootPath ?? EnvironmentConfig.Current.ContentRootPath;

            if (options.StaticFilesRequestPaths.Is())
            {
                foreach (var staticFilePath in options.StaticFilesRequestPaths)
                {
                    if (staticFilePath == null) continue;

                    StaticFileOptions staticFileOptions = new StaticFileOptions
                    {
                        ServeUnknownFileTypes = options.StaticFilesServeUnknownFileTypes,
                        HttpsCompression = HttpsCompressionMode.Compress,
                        RedirectToAppendTrailingSlash = false,
                        OnPrepareResponse = ctx =>
                        {
                            if (ctx.Context.Response.Headers.ContainsKey("Cache-Control") != true)
                                ctx.Context.Response.Headers.Append("Cache-Control", $"public, max-age=" + options.StaticFilesMaxAgeSeconds);
                        },
                    };

                    staticFileOptions.FileProvider = new PhysicalFileProvider(contentRootPath);
                    staticFileOptions.RequestPath = new PathString(staticFilePath);
                    app.UseStaticFiles(staticFileOptions);
                }
            }
            else
            {
                StaticFileOptions staticFileOptions = new StaticFileOptions
                {
                    ServeUnknownFileTypes = options.StaticFilesServeUnknownFileTypes,
                    HttpsCompression = HttpsCompressionMode.Compress,
                    RedirectToAppendTrailingSlash = false,
                    OnPrepareResponse = ctx =>
                    {
                        if (ctx.Context.Response.Headers.ContainsKey("Cache-Control") != true)
                            ctx.Context.Response.Headers.Append("Cache-Control", $"public, max-age=" + options.StaticFilesMaxAgeSeconds);
                    },
                };
                staticFileOptions.FileProvider = new PhysicalFileProvider(contentRootPath);
                staticFileOptions.RequestPath = new PathString("");
                app.UseStaticFiles(staticFileOptions);
            }
        }

        if (options.UseRouting)
            app.UseRouting();

        if (options.UseCookiePolicy)
            app.UseCookiePolicy();

        if (options.UseOutputCache && !options.UseOutputCacheAfterAuthentication)
            app.UseOutputCache();

        if (!options.UseOutputCacheAfterAuthentication)
        {
            if (options.UseBrotliResponseCompression || options.UseGzipResponseCompression)
            {
                app.UseWhen((context) => Compress.IsEligibleForCompression(context, options), appCompression =>
                {
                    appCompression.UseResponseCompression();
                });
            }
        }

        if (options.UseAuthentication)
            app.UseAuthentication();

        if (options.UseOutputCache && options.UseOutputCacheAfterAuthentication)
            app.UseOutputCache();

        if (options.UseOutputCacheAfterAuthentication)
        {
            if (options.UseBrotliResponseCompression || options.UseGzipResponseCompression)
            {
                app.UseWhen((context) => Compress.IsEligibleForCompression(context, options), appCompression =>
                {
                    appCompression.UseResponseCompression();
                });
            }
        }

        if (options.UseAuthorization)
            app.UseAuthorization();

        if (options.PrecededEndpoints != null)
        {
            app.UseEndpoints(endpoints => options.PrecededEndpoints(endpoints));
        }

        if (options.UseControllers)
        {
            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
                endpoints.MapControllerRoute("api/{controller}/{action}/{id?}", "api/{controller}/{action}/{id?}");
            });
        }

        if (options.UseRazorPages)
            app.UseEndpoints(endpoints => endpoints.MapRazorPages());

        var enablePrometheusMetrics = AppSettings.Current?.SystemLibraryCommonFramework?.Metrics?.EnablePrometheus;
        if (enablePrometheusMetrics == true)
        {
            app.UseEndpoints(endpoints =>
            {
                Debug.Log("[IApplicationBuilder] Adding /metrics and /metrics/ endpoints");

                Metrics.SuppressDefaultMetrics();

                endpoints.MapGet("/metrics", async context =>
                {
                    if (!MetricsAuthorizationMiddleware.AuthorizeMetricsRequest(context))
                    {
                        Debug.Log("[MetricsAuthorizationMiddleware] not authorized");
                        return;
                    }

                    Debug.Log("[MetricsAuthorizationMiddleware] reading metrics...");

                    try
                    {
                        await Metrics.DefaultRegistry.CollectAndExportAsTextAsync(context.Response.Body);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);

                        throw;
                    }
                });
            });
        }

        return app;
    }
}