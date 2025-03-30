using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

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

        app.Use(BlacklistedRequestMiddleware.Use);

        if (options.UseDeveloperPage)
            app.UseDeveloperExceptionPage();

        if (options.UseForwardedHeaders)
            app.UseForwardedHeaders();

        if (options.UseHttpsRedirection)
            app.UseHttpsRedirection();

        if (options.UseStaticFilePolicy)
            UseStaticFilePolicy(app, env, options);

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

        if (options.UseHsts)
            app.UseHsts();

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

        UseMetrics(app);

        return app;
    }
}