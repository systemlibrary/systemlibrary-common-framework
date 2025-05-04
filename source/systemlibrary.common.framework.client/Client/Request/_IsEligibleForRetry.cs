using System.Net;
using System.Net.Sockets;

namespace SystemLibrary.Common.Framework.App;

partial class Client
{
    partial class Request
    {
        /// <summary>
        /// Request is eligible for a retry if status code is 502 or 504 and url is not a file request or if socket was forcibly closed by remote host
        /// UseRetryPolicy: 
        /// False: retries one time on 502, 504 for GET, POST and file requests, and on forcibly closed socket connection by remote host
        /// True: same as 'false', but adds:
        /// - retries one additional time on 502, 504 GET, POST
        /// - retries up to two times if response is null
        /// - retries once on 401 GET, POST
        /// - retries once on 404 GET
        /// - retries once on 500 GET, POST
        /// - retries once on 404, 500, 502 504 file request
        /// - retries once on OPTION, PATCH, HEAD, CONNECT, TRACE
        /// </summary>
        static bool IsEligibleForRetry(RequestOptions options, HttpResponseMessage response, int retry, Exception ex)
        {
            if (ex != null)
            {
                // A connection is forcibly closed by the remote host, for some reason, lets retry
                // It might be a valid closed by force, as in too many requests, so remote host denies us, but we only retry 1 time
                if (ex is HttpRequestException || ex is SocketException)
                {
                    // One retry only
                    if (ex.Message.Contains("forcibly closed by") == true ||
                        ex.InnerException?.Message.Contains("forcibly closed by") == true ||
                        ex.InnerException?.InnerException?.Message.Contains("forcibly closed by") == true)
                    {
                        Log.Debug(options.Url + " " + ex.Message);

                        return retry == 0;
                    }
                }
            }

            if (!options.UseRetryPolicy)
            {
                if (response != null)
                {
                    if (options.Method != HttpMethod.Get && options.Method != HttpMethod.Post && !options.Url.IsFile())
                        return false;

                    return
                        response.StatusCode == HttpStatusCode.BadGateway ||
                        response.StatusCode == HttpStatusCode.GatewayTimeout;
                }

                return false;
            }

            HttpMethod method = options.Method;

            return
                IsResponseEligibleForRetry(response) &&
                IsRequestEligibleForRetry(options, method, response?.StatusCode, retry);
        }

        static bool IsRequestEligibleForRetry(RequestOptions options, HttpMethod method, HttpStatusCode? responseStatusCode, int retry)
        {
            if (responseStatusCode == HttpStatusCode.InternalServerError)
            {
                // FileRequest: 0 retries on 500
                if (options.Url.IsFile()) return false;

                // GET: 1 retry on 500
                return (method == HttpMethod.Get) && retry == 0;
            }

            if (responseStatusCode == HttpStatusCode.Unauthorized)
            {
                // GET POST: 1 retry on 401
                return (method == HttpMethod.Get || method == HttpMethod.Post) && retry == 0;
            }

            // FileRequest: 1 retry
            if (options.Url.IsFile()) return retry == 0;

            if (method == HttpMethod.Delete || method == HttpMethod.Put)
            {
                // DELETE, PUT: 0 retries
                return false;
            }
            else if (method == HttpMethod.Get)
            {
                // GET 404: 1 retry
                if (responseStatusCode == HttpStatusCode.NotFound) return retry == 0;

                // GET: 2 retries
                return true;
            }

            // OPTION, PATCH, HEAD, CONNECT, TRACE, POST: 1 retry
            return retry == 0;
        }

        static bool IsResponseEligibleForRetry(HttpResponseMessage response)
        {
            var statusCode = response?.StatusCode;

            if (statusCode == null)
            {
                return true;
            }

            return
                statusCode == HttpStatusCode.InternalServerError ||
                statusCode == HttpStatusCode.Unauthorized ||
                statusCode == HttpStatusCode.BadGateway ||
                statusCode == HttpStatusCode.GatewayTimeout;
        }
    }
}
