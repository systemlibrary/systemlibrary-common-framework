namespace SystemLibrary.Common.Framework.App.Extensions;

/// <summary>
/// Web Application Builder Options
/// <para>All options are 'true' (on) by default</para>
/// Used as argument in 'app.CommonWebApplicationBuilder' function
/// </summary>
/// <example>
/// Startup.cs/Program.cs:
/// <code>
/// public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
/// {
///     var options = new FrameworkAppOptions();
///     
///     options.UseHttpRedrectionAndHsts = false;
///     
///     app.CommonWebApplicationBuilder(options);
/// }
/// </code>
/// </example>
public class FrameworkAppOptions : FrameworkOptions
{
    /// <summary>
    /// Adds middleware for http to https redirect client side, aka hsts
    /// </summary>
    public bool UseHsts = true;

    /// <summary>
    /// Adds the routing middleware that comes with Aspnet
    /// </summary>
    public bool UseRouting = true;

    /// <summary>
    /// Adds middleware for Authorization and Authentication attributes
    /// </summary>
    public bool UseAuthentication = true;

    /// <summary>
    /// Adds middleware for Authorization attributes
    /// </summary>
    public bool UseAuthorization = true;

    /// <summary>
    /// Adds middleware for static files and sets a few default settings:
    /// <para>- allows serving of unknown files types</para>
    /// - compression is set to 'HttpsCompressionMode.Compress'
    /// <para>- does not append a trailing slash for static files</para>
    /// </summary>
    public bool UseStaticFilePolicy = true;

    /// <summary>
    /// Set the cache-control max age header to a duration for all static requests
    /// <para>Default: 20 days</para>
    /// </summary>
    /// <remarks>
    /// Requires UseStaticFiles set to True, and the header 'max-age' cannot be added already in the response
    /// </remarks>
    public int StaticFilesMaxAgeSeconds = 1728000;

    /// <summary>
    /// Allow serving of unknown, unsupported, media/mime types.
    /// <para>Defaults to true</para>
    /// </summary>
    /// <remarks>
    /// Requires UseStaticFiles set to True
    /// </remarks>
    public bool StaticFilesServeUnknownFileTypes = true;

    /// <summary>
    /// Set the relative paths of where most static content is served from
    /// <para>For example: new string[] { "/static", "/public" }</para>
    /// <para>This requires either you set env.WebRootPath before invoking the Options or that the built-in root path EnvironmentConfig.Current.ContentRootPath is what you want</para>
    /// </summary>
    /// <remarks>
    /// Requires UseStaticFiles set to True
    /// </remarks>
    public string[] StaticFilesRequestPaths = null;

    /// <summary>
    /// Adds middleware which responds with a exception page usually used in development environments and test environments
    /// </summary>
    public bool UseDeveloperPage = true;
}
