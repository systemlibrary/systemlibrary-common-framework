using System;
using System.Net.Http;
using System.Threading.Tasks;

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
                    if (!IsEligibleForRetry(options, response, retry, ex))
                    {
                        if (EnablePrometheusConfig)
                            IncrementMetric(options, response, retry);

                        // Response is success or no more retries so we break
                        break;
                    }

                    // Debug.Log("Retry count: " + (retry + 1) + " " + options.Url + ": " + response?.StatusCode);

                    ex = null;
                    response = null;
                    await Task.Delay(TimeSpan.FromMilliseconds(666)).ConfigureAwait(false);
                }
                else
                {
                    if (EnablePrometheusConfig)
                        IncrementMetric(options, response, retry);
                }
            }

            return (response, ex);
        }

        static void IncrementMetric(RequestOptions options, HttpResponseMessage response, int retry)
        {
            if (response?.IsSuccessStatusCode == true)
            {
                var statusCode = (int)(response?.StatusCode ?? 0);

                var status = retry == 0 ? "success" : "retry_success";

                var statusCodeLabel = (statusCode == 301 || statusCode == 302) ? statusCode.ToString() : "200";

                ClientRequestCounter.WithLabels(options.UriLabel, status, statusCodeLabel).Inc();
            }
            else
            {
                ClientRequestCounter.WithLabels(options.UriLabel, "failed", response?.StatusCode.ToString() ?? "0").Inc();
            }
        }
    }
}
