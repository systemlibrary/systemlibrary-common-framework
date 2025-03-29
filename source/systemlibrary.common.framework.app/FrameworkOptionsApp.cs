namespace SystemLibrary.Common.Framework;

partial class FrameworkOptions
{
    /// <summary>
    /// Adds middleware which responds with a exception page usually used in development environments and test environments
    /// </summary>
    public bool UseDeveloperPage = true;

    /// <summary>
    /// Adds middleware for static files and sets a few default settings:
    /// <para>- allows serving of unknown files types</para>
    /// - compression is set to 'HttpsCompressionMode.Compress'
    /// <para>- does not append a trailing slash for static files</para>
    /// </summary>
    public bool UseStaticFilePolicy = true;

    /// <summary>
    /// Set the cache-control max age header to a duration for all static requests
    /// <para>Default: 14 days</para>
    /// </summary>
    /// <remarks>
    /// Requires UseStaticFiles set to True, and the header 'max-age' cannot be added already in the response, if so this does nothing
    /// </remarks>
    public int StaticFilesClientCacheSeconds = 1209600;

    /// <summary>
    /// Set the relative paths of where most static content is served from
    /// <para>For example: new string[] { "/static", "/public" }</para>
    /// <para>This requires either you set env.WebRootPath before invoking the Options or that the built-in root path EnvironmentConfig.Current.ContentRootPath is what you want</para>
    /// </summary>
    /// <remarks>
    /// Requires UseStaticFiles set to True
    /// </remarks>
    public string[] StaticRequestPaths = null;

}
