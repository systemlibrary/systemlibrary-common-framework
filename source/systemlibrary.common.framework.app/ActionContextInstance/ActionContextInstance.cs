using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace SystemLibrary.Common.Framework.App;

/// <summary>
/// An implementation of the 'old' thread safe singleton ActionContext we all know and love from .NET Framework
/// </summary>
public static class ActionContextInstance
{
    internal static IActionContextAccessor ActionContextAccessor;

    /// <summary>
    /// Get the current Action Context instance
    /// </summary>
    /// <remarks>
    /// Do note that Action Context can be null in a console application or in a 'Unit' Test Application or if MVC is not yet registered as a service/invoked
    /// </remarks>
    /// <example>
    /// <code>
    /// var actionContext = ActionContextInstance.Current;
    /// </code>
    /// </example>
    /// <return>Returns current Action Context or null if there is none</return>
    public static Microsoft.AspNetCore.Mvc.ActionContext Current
    {
        get
        {
            var ctx1 = ActionContextAccessor?.ActionContext;
            var ctx2 = ActionContextAccessor?.ActionContext;

            return ctx1 == ctx2 ? ctx1 : null;
        }
    }
}
