using Microsoft.AspNetCore.Http;

namespace SystemLibrary.Common.Framework;

internal static class MetricsAuthorizationMiddleware
{
    public static bool AuthorizeMetricsRequest(HttpContext context)
    {
        var authorizationValue = FrameworkConfigInstance.Current.Metrics.AuthorizationValue;
        var authorization = context.Request.Headers["Authorization"].ToString();

        if (authorizationValue.IsNot() ||
            "Basic " + authorizationValue == authorization ||
            authorizationValue == authorization)
        {
            Debug.Log("[MetricsMiddleware] 200 Authorized");

            return true;
        }

        // Give user option to prompt for an Auth?
        //context.Response.Headers["WWW-Authenticate"] = "Basic";
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.WriteAsync(StatusCodes.Status401Unauthorized.ToString() + ": Metric endpoint requires access through the Authorization header. The value required is set by the application developers.")
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

        Debug.Log("[MetricsMiddleware] 401 Unauthorized");

        return false;
    }
}