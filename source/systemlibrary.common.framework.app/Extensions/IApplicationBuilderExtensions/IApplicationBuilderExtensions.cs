using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.FileProviders;

using Prometheus;

using SystemLibrary.Common.Framework.Extensions;
using SystemLibrary.Common.Framework.Licensing;

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
    ///     var options = new FrameworkOptions();
    ///     
    ///     app.UseFrameworkApp(options);
    /// }
    /// </code>
    /// </example>
    public static IApplicationBuilder UseFrameworkMiddlewares(this IApplicationBuilder app, IWebHostEnvironment env, FrameworkOptions options = null)
    {
        ServiceProviderInstance.Instance = app.ApplicationServices;

        options ??= new FrameworkOptions();

        if (options.UseDeveloperPage)
            app.UseDeveloperExceptionPage();

        app.Use(BlacklistedRequestMiddleware.Use);

        if (options.UseForwardedHeaders)
            app.UseForwardedHeaders();

        if (options.UseHttpsRedirection)
        {
            app.UseHttpsRedirection();
        }

        if (options.UseStaticFilePolicy)
        {
            var contentRootPath = env?.WebRootPath ?? EnvironmentConfig.ContentRootPath;

            if (options.StaticFilesRequestPaths.Is())
            {
                foreach (var staticFilePath in options.StaticFilesRequestPaths)
                {
                    if (staticFilePath == null) continue;

                    StaticFileOptions staticFileOptions = new StaticFileOptions
                    {
                        ServeUnknownFileTypes = true,
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
                    ServeUnknownFileTypes = true,
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

        if (options.UseCookiePolicy)
            app.UseCookiePolicy();

        app.UseRouting();

        if (options.UseAuthentication)
            app.UseAuthentication();

        if (options.UseOutputCache)
            app.UseOutputCache();

        if (options.UseAuthorization)
            app.UseAuthorization();

        if (options.BeforeDefaultEndpoints != null)
            app.UseEndpoints(endpoints => options.BeforeDefaultEndpoints(endpoints));

        if (options.UseControllers)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                //endpoints.MapControllerRoute("api/{controller}/{action}/{id?}", "api/{controller}/{action}/{id?}");
            });
        }

        if (options.UseHttpsRedirection)
        {
            app.UseHsts();
        }

        if (options.UseMvc)
            app.UseEndpoints(endpoints => endpoints.MapRazorPages());

        if (options.AfterDefaultEndpoints != null)
            app.UseEndpoints(endpoints => options.AfterDefaultEndpoints(endpoints));

        if (options.UseBrotliResponseCompression || options.UseGzipResponseCompression)
        {
            app.UseWhen((context) => Compress.IsEligibleForCompression(context, options), appCompression =>
            {
                appCompression.UseResponseCompression();
            });
        }

        var enablePrometheusMetrics = AppSettings.Current.SystemLibraryCommonFramework.Metrics.Enable;
        if (enablePrometheusMetrics)
        {
            if (License.Gold())
            {
                app.UseEndpoints(endpoints =>
                {
                    Debug.Log("[IApplicationBuilder] Adding /metrics endpoint");

                    Metrics.SuppressDefaultMetrics();

                    endpoints.MapGet("/metrics", async context =>
                    {
                        if (!MetricsAuthorizationMiddleware.AuthorizeMetricsRequest(context))
                        {
                            Debug.Log("[MetricsAuthorizationMiddleware] 401 Unauthorized");
                            return;
                        }

                        Debug.Log("[MetricsAuthorizationMiddleware] 200 Authorized");

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
        }

        return app;
    }
}