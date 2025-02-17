using Microsoft.AspNetCore.Routing;

namespace SystemLibrary.Common.Framework.App.Extensions;

public abstract class BaseOptions
{
    /// <summary>
    /// Set to true to add services and middleware for controllers and api controllers
    /// </summary>
    public bool UseControllers = true;

    /// <summary>
    /// Set to true to add MVC services
    /// <para>NOTE: This also registers the controllers even if you try to set UseControllers to false</para>
    /// <para>NOTE: This registers a default media type output formatter, so all types are allowed served with exception of: .cs, .exe, .dll, .config, .iso, .dmg and a few more...</para>
    /// </summary>
    public bool UseMvc = true;

    /// <summary>
    /// Optional: Additional endpoints configuration that is registered in front of RazorPages, Controllers and ApiControllers
    /// </summary>
    /// <remarks>
    /// These endpoints will be called after StaticFiles, Routing, CookiePolicy, OutputCache, Authentication and Authorization, but before MVC
    /// </remarks>
    public Action<IEndpointRouteBuilder> BeforeDefaultEndpoints = null;

    /// <summary>
    /// Optional: Additional endpoints configuration that is registered in after RazorPages, Controllers and ApiControllers
    /// </summary>
    public Action<IEndpointRouteBuilder> AfterDefaultEndpoints = null;

    /// <summary>
    /// Set to true to add services and middleware for cookie policies
    /// </summary>
    public bool UseCookiePolicy = true;

    /// <summary>
    /// Set to true to add services and middleware for forwarded headers
    /// </summary>
    public bool UseForwardedHeaders = true;

    /// <summary>
    /// Set to true to add services and middleware to use http to https redirection
    /// </summary>
    public bool UseHttpsRedirection = true;

    /// <summary>
    /// Set to true to register services and middleware for the OutputCache in ASPNET
    /// </summary>
    public bool UseOutputCache = true;

    /// <summary>
    /// Set to true to register services and middleware for the OutputCache in ASPNET after the Authentication, so Authentication always triggers before checking output cache
    /// <para>This is the output cache middleware from microsoft, completely different cache than the class Cache in this library</para>
    /// </summary>
    public bool UseOutputCacheAfterAuthentication = true;

    /// <summary>
    /// Set to true to add services and middleware Gzip compression
    /// </summary>
    public bool UseGzipResponseCompression = true;

    /// <summary>
    /// Set to true to add services and middleware Gzip compression
    /// </summary>
    public bool UseBrotliResponseCompression = false;
}