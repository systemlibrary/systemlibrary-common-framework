using Microsoft.AspNetCore.Http;

namespace SystemLibrary.Common.Framework;

internal static class MetricsAuthorizationMiddleware
{
    public static bool AuthorizeMetricsRequest(HttpContext context)
    {
        var metricToken = FrameworkConfigInstance.Current.Metrics.MetricUIToken;

        var token = context.Request.Headers["metricUIToken"].ToString();

        if (metricToken.IsNot() ||
            metricToken == token)
        {
            Debug.Log("[Metrics] authorized");

            return true;
        }

        // Give user option to prompt for an Auth?
        //context.Response.Headers["WWW-Authenticate"] = "Basic";
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.WriteAsync(StatusCodes.Status401Unauthorized.ToString() + ": Metric endpoint requires access through the 'metricUIToken' header. The value required is set by the application developers.")
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

        Debug.Log("[MetricsMiddleware] unauthorized");

        return false;
    }
}