using Microsoft.AspNetCore.Http;

namespace SystemLibrary.Common.Framework;

/// <summary>
/// An implementation of the 'old' thread safe singleton HttpContext we all know and love from .NET Framework
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
    public static HttpContext Current
    {
        get
        {
            var ctx1 = HttpContextAccessor?.HttpContext;

            if (ctx1 == null) return new DefaultHttpContext();

            var ctx2 = HttpContextAccessor?.HttpContext;

            return ctx1 == ctx2 ? ctx1 : new DefaultHttpContext();
        }
    }
}
