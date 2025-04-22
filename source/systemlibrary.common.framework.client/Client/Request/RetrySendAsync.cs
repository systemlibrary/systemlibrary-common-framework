using System.Net;

using SystemLibrary.Common.Framework.Licensing;

namespace SystemLibrary.Common.Framework.App;

partial class Client
{
    partial class Request
    {
        internal static async Task<(HttpResponseMessage, Exception)> RetrySendAsync(RequestOptions options)
        {
            var maxRetries = options.UseRetryPolicy ? 3 : 2;

            HttpResponseMessage response = null;
            Exception ex = null;

            for (int retry = 0; retry < maxRetries; retry++)
            {
                options.Update(retry);
                try
                {
                    response = await SendAsync(options);
                }
                catch (ArgumentException arg)
                {
                    ex = arg;
                    break;
                }
                catch (InvalidOperationException invalid)
                {
                    ex = invalid;
                    break;
                }
                catch (IndexOutOfRangeException index)
                {
                    ex = index;
                    break;
                }
                catch (NullReferenceException noref)
                {
                    ex = noref;
                    break;
                }
                catch (Exception e)
                {
                    ex = e;
                }
                if (options.CancellationToken.IsCancellationRequested)
                {
                    ex = new CalleeCancelledRequestException("Callee cancelled request to " + options.Url, ex);
                    break;
                }

                if (retry != maxRetries - 1)
                {
                    IncrementMetric(options, response, retry);

                    if (!IsEligibleForRetry(options, response, retry, ex))
                    {
                        // Response is success or no more retries so we break
                        break;
                    }

                    ex = null;
                    response = null;
                    await Task.Delay(TimeSpan.FromMilliseconds(666)).ConfigureAwait(false);
                }
                else
                {
                    IncrementMetric(options, response, retry);
                }
            }

            return (response, ex);
        }

        static void IncrementMetric(RequestOptions options, HttpResponseMessage response, int retry)
        {
            if (!EnablePrometheusConfig) return;

            if (!License.Gold()) return;

            if (response?.IsSuccessStatusCode == true)
            {
                var statusCode = (int)(response?.StatusCode ?? 0);

                var status = retry == 0 ? "success" : "retry_success";

                var statusCodeLabel = (statusCode == 301 || statusCode == 302) ? statusCode.ToString() : "200";

                Metric.Inc(options.UriLabel, status + ":" + statusCodeLabel);
            }
            else
            {
                Metric.Inc(options.UriLabel, "failed:" + ((int)(response?.StatusCode ?? (HttpStatusCode)504)).ToString());
            }
        }
    }
}
