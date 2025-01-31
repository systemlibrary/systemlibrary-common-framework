using Microsoft.AspNetCore.Http;

namespace SystemLibrary.Common.Framework;

/// <summary>
/// A class that has a reference to the HttpContext instance
/// </summary>
public static class HttpContextInstance
{
    internal static IHttpContextAccessor HttpContextAccessor;

    /// <summary>
    /// Get the current Http Context instance
    /// </summary>
    /// <remarks>
    /// Do note that Http Context can be null in a console application or in a 'Unit' Test Application or if MVC is not yet registered as a service/invoked
    /// </remarks>
    /// <example>
    /// <code>
    /// var httpContext = HttpContextInstance.Current;
    /// </code>
    /// </example>
    /// <return>Returns current Http Context or null if there is none</return>
    public static HttpContext Current => HttpContextAccessor?.HttpContext;
}
