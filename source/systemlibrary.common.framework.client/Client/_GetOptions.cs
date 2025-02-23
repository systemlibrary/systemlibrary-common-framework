using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;

namespace SystemLibrary.Common.Framework.App;

partial class Client
{
    RequestOptions GetRequestOptions(HttpMethod method, string url, object data, MediaType mediaType, int timeout, IDictionary<string, string> headers, JsonSerializerOptions jsonSerializerOptions, CancellationToken cancellationToken)
    {
        var u = new System.Uri(url);

        return new RequestOptions()
        {
            Method = method,
            Url = url,
            Uri = u,
            UriLabel = GetUriLabel(this.GetType().Name, u),
            Headers = headers,
            MediaType = mediaType,
            Data = data,
            JsonSerializerOptions = jsonSerializerOptions,

            ForceNewClient = false,

            CancellationToken = cancellationToken,

            UseRetryPolicy = UseRetryPolicy,
            IgnoreSslErrors = IgnoreSslErrors,

            Timeout = GetTimeout(timeout),
            RetryTimeout = RetryTimeout,
        };
    }

    int GetTimeout(int timeout)
    {
        // Not passed in a custom one 
        // Edge case: it might be passed in exactly the same so that would change the timeout
        if (timeout == DefaultTimeout)
        {
            return Timeout;
        }

        if (timeout <= 0) return Timeout;

        return timeout;
    }
}
