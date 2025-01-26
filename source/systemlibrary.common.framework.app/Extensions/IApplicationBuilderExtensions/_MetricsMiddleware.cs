using Microsoft.AspNetCore.Http;

using SystemLibrary.Common.Framework;

internal static class MetricsAuthorizationMiddleware
{
    public static bool AuthorizeMetricsRequest(HttpContext context)
    {
        var authorizationValue = FrameworkConfig.Current.Metrics.AuthorizationValue;
        var authorization = context.Request.Headers["Authorization"].ToString();

        if(authorizationValue.IsNot() || "Basic " + authorizationValue == authorization)
        {
            return true;
        }

        context.Response.Headers["WWW-Authenticate"] = "Basic";
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.WriteAsync(StatusCodes.Status401Unauthorized.ToString())
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

        return false;
    }
}