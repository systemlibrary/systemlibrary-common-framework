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

        var err = (string)null;

        var metricToken = FrameworkConfigInstance.Current.Metrics.MetricUIToken;

        for (int i = 0; i < 6; i++)
        {
            try
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(7500));
                using var client = new HttpClient();

                client.DefaultRequestHeaders.Add("slcf-metrics-ui", "true");

                if (metricToken.Is())
                    client.DefaultRequestHeaders.Add("metricUIToken", metricToken);

                var response = client.GetStringAsync(url, cts.Token)
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult();

                if (response?.Length > 10)
                {
                    sb.AppendLine("\n");
                    sb.Append(response);
                }
            }
            catch (Exception ex)
            {
                err = ex.ToString();
            }
        }

        if (err != null)
        {
            Log.Error("[MetricsFetcher] exception occured: " + err);
        }

        return sb.ToString();
    }
}
