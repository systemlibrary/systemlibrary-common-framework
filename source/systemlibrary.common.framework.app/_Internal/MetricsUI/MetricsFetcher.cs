using System.Text;

using Microsoft.AspNetCore.Http;

namespace SystemLibrary.Common.Framework;

internal static class MetricsFetcher
{
    internal static string Get(HttpContext context)
    {
        var url = $"{context.Request.Scheme}://{context.Request.Host}/metrics";

        var sb = new StringBuilder("");

        var err = (string)null;
        for (int i = 0; i < 7; i++)
        {
            try
            {
                using HttpClient client = new HttpClient();

                client.DefaultRequestHeaders.Add("metrics-ui", "true");

                var response = client.GetStringAsync(url)
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
