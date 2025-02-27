using System.Net;
using System.Text.Json;

using Polly.CircuitBreaker;

namespace SystemLibrary.Common.Framework.App;

partial class Client
{
    async Task<ClientResponse<T>> SendAsync<T>(HttpMethod method, string url, object data, ContentType contentType, int timeout, IDictionary<string, string> headers, JsonSerializerOptions jsonSerializerOptions, CancellationToken cancellationToken, Func<string, T> deserialize)
    {
        if (url.IsNot())
            throw new Exception("Url is missing when trying to make a " + method + " request");

        var options = GetRequestOptions(method, url, data, contentType, timeout, headers, jsonSerializerOptions, cancellationToken);

        HttpResponseMessage response = null;
        Exception ex = null;

        var useRequestBreakerPolicy = IsEligibleForRequestBreakerPolicy(options);

        if (useRequestBreakerPolicy)
            (response, ex) = await RetrySendWithRequestBreakerAsync(options).ConfigureAwait(false);
        else
            (response, ex) = await Request.RetrySendAsync(options).ConfigureAwait(false);

        var isSuccess = response?.IsSuccessStatusCode == true;

        if (ex != null || !isSuccess)
        {
            if (ex is BrokenCircuitException bce)
            {
                var policyKey = PolicyKeyConverter.Convert(options);
                var message = GetRequestErrorMessage(options).TrimEnd() +
                    ", " + RequestBreaker.BreakOnExceptionsInRow + " exceptions in a row, request for client + path (" + policyKey + ") is stopped for " +
                    RequestBreaker.BrokenDuration + " seconds. " +
                    bce.Message +
                    " Reason: " + bce.InnerException?.Message;

                ex = new BrokenCircuitException(message);
            }

            if (ThrowOnUnsuccessful)
            {
                if (ex is CalleeCancelledRequestException)
                    throw ex;

                if (ex is BrokenCircuitException)
                    throw ex;

                throw GetHttpRequestException(options, response, ex);
            }
            else
            {
                if (ex != null)
                {
                    if (ex is BrokenCircuitException)
                        Log.Warning(ex);

                    else if (response == null)
                    {
                        Log.Warning(GetRequestErrorMessage(options) + " " + ex.Message);
                    }
                }
            }
        }

        var type = typeof(T);

        if (deserialize == null)
        {
            var responseData = await ReadResponseAsync<T>(type, url, response, cancellationToken, jsonSerializerOptions).ConfigureAwait(false);

            return new ClientResponse<T>(response, responseData, ex?.Message);
        }
        else
        {
            var responseString = await ReadResponseAsync<string>(SystemType.StringType, url, response, cancellationToken, jsonSerializerOptions).ConfigureAwait(false);

            return new ClientResponse<T>(response, deserialize(responseString), ex?.Message);
        }
    }

    async Task<(HttpResponseMessage, Exception)> RetrySendWithRequestBreakerAsync(RequestOptions options)
    {
        var policyKey = PolicyKeyConverter.Convert(options);

        var requestBreaker = RequestBreaker.GetPolicy(policyKey);
        HttpResponseMessage response = null;
        Exception ex = null;

        try
        {
            return await requestBreaker.ExecuteAsync(async () =>
            {
                (response, ex) = await Request.RetrySendAsync(options).ConfigureAwait(false);

                // Check, create and throw an exception to trigger circuit breaker
                if (ex == null && (response == null ||
                    response.StatusCode == HttpStatusCode.NotFound ||
                    response.StatusCode == HttpStatusCode.TooManyRequests ||
                    response.StatusCode == HttpStatusCode.InternalServerError ||
                    response.StatusCode == HttpStatusCode.BadGateway ||
                    response.StatusCode == HttpStatusCode.ServiceUnavailable ||
                    response.StatusCode == HttpStatusCode.GatewayTimeout
                    ))
                {
                    ex = GetHttpRequestException(options, response);
                    throw ex;
                }

                return (response, ex);
            }).ConfigureAwait(false);
        }
        catch (BrokenCircuitException bce)
        {
            if (EnablePrometheusConfig)
                ClientRequestCounter.WithLabels(options.UriLabel, "circuit_broken", "0").Inc();

            return (response, bce);
        }
        catch (Exception e)
        {
            return (response, ex ?? e?.InnerException ?? e);
        }
    }
}
