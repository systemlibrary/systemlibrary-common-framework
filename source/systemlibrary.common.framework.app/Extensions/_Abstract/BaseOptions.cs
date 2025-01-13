using Microsoft.AspNetCore.Routing;

namespace SystemLibrary.Common.Framework.App.Extensions;

public abstract class BaseOptions
{
    /// <summary>
    /// Set to true to add services and middleware for razor pages
    /// <para>NOTE: This also registers a default media type output formatter, so your application is allowed to serve default mime types, such as: html, css, js, jpg, png, tiff, woff, json, xml, and a few others</para>
    /// </summary>
    public bool UseRazorPages = true;

    /// <summary>
    /// Set to true to add services and middleware for controllers and api controllers
    /// </summary>
    public bool UseControllers = true;

    /// <summary>
    /// Optional: Additional endpoints configuration that is registered in front of RazorPages, Controllers and ApiControllers
    /// </summary>
    public Action<IEndpointRouteBuilder> PrecededEndpoints = null;

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