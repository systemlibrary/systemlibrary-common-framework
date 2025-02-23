using Microsoft.AspNetCore.Routing;

namespace SystemLibrary.Common.Framework;

/// <summary>
/// Framework service and middelware options
/// </summary>
/// <remarks>
/// </remarks>
/// <example>
/// Inside your startup.cs/program.cs...
/// <code>
/// FrameworkOptions Options = new FrameworkOptions { };
/// 
/// public class CustomViewLocations : IViewLocationExpander
/// {
///     //...implement the interface
///     public IEnumerable&lt;string&gt; ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable&lt;string&gt; viewLocations)
///     {
///         return new string[] {   
///             "~/Folder2/{0}/Index.cshtml"
///         }
///     }
/// }
/// 
/// public void ConfigureServices(IServiceCollection services)
/// {
///     Options.UseMvc = false;
///     // Options.ViewLocationExpander = new CustomViewLocations();
///     Options.ViewLocations = new string[] {
///         "~/Folder/{0}/Index.cshtml",
///         "~/Folder/{1}/{0}.cshtml"
///     }
///     app.AddFrameworkServices(Options);
/// }
/// 
/// public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
/// {
///     Options.UseHttpRedrectionAndHsts = false;
///     
///     app.UseFrameworkMiddlewares(Options);
/// }
/// </code>
/// </example>
public partial class FrameworkOptions
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
    /// Optional: Additional endpoints configuration that is registered after RazorPages, Controllers and ApiControllers, but before Compression
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
    /// Set to true to register services and middleware for the OutputCache
    /// </summary>
    public bool UseOutputCache = true;

    /// <summary>
    /// Set to true to add services and middleware Gzip compression
    /// </summary>
    public bool UseGzipResponseCompression = true;

    /// <summary>
    /// Set to true to add services and middleware Gzip compression
    /// </summary>
    public bool UseBrotliResponseCompression = false;
}
