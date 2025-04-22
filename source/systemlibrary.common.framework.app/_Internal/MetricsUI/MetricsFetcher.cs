using System.Text;

using Microsoft.AspNetCore.Http;

namespace SystemLibrary.Common.Framework;

internal static class MetricsFetcher
{
    internal static string Get(HttpContext context)
    {
        var origin = context.Request.Headers["Origin"].FirstOrDefault();

        var forwardedHost = context.Request.Headers["X-Forwarded-Host"].FirstOrDefault();

        var host = forwardedHost ?? origin ?? context.Request.Host.Value;

        var url = $"{context.Request.Scheme}://{host}/metrics";

        var sb = new StringBuilder("");

        var metricToken = FrameworkConfigInstance.Current.Metrics.MetricUIToken;

        var responses = Async.Parallel<string>(
            () => GetMetricsResponse(url, metricToken),
            () => GetMetricsResponse(url, metricToken),
            () => GetMetricsResponse(url, metricToken),
            () => GetMetricsResponse(url, metricToken),
            () => GetMetricsResponse(url, metricToken),
            () => GetMetricsResponse(url, metricToken)
        );

        if (responses != null)
        {
            foreach (var response in responses)
            {
                if (response?.Length > 10)
                {
                    sb.AppendLine("\n");
                    sb.Append(response);
                }
            }
        }

        return sb.ToString();
    }

    static string GetMetricsResponse(string url, string metricToken)
    {
        var handler = new SocketsHttpHandler
        {
            UseProxy = false
        };
        var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(5000));
        var client = new HttpClient(handler);

        client.Timeout = TimeSpan.FromMilliseconds(5000);

        client.DefaultRequestHeaders.Add("slcf-metrics-ui", "true");

        if (metricToken.Is())
            client.DefaultRequestHeaders.Add("metricUIToken", metricToken);

        try
        {
            return
                client.GetStringAsync(url)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult()
            ;
        }
        catch (Exception ex)
        {
            Log.Error("[MetricsFetcher] exception occured: " + ex.Message);
            return null;
        }
    }
}
