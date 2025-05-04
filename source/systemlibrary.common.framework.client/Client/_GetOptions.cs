using System.Text.Json;

namespace SystemLibrary.Common.Framework.App;

partial class Client
{
    RequestOptions GetRequestOptions(HttpMethod method, string url, object data, ContentType contentType, int timeout, IDictionary<string, string> headers, JsonSerializerOptions jsonSerializerOptions, CancellationToken cancellationToken)
    {
        var u = new System.Uri(url);

        return new RequestOptions()
        {
            Method = method,
            Url = url,
            Uri = u,
            UriLabel = GetUriLabel(this.GetType().Name, u),
            Headers = headers,
            ContentType = contentType,
            Data = data,
            JsonSerializerOptions = jsonSerializerOptions,

            ForceNewClient = false,

            CancellationToken = cancellationToken,
            ExpectContinue = ExpectContinue,
            UseRetryPolicy = UseRetryPolicy,
            UseAutomaticDecompression = UseAutomaticDecompression,

            IgnoreSslErrors = IgnoreSslErrors,

            Timeout = GetTimeout(timeout),
            RetryTimeout = RetryTimeout,
        };
    }

    int GetTimeout(int timeout)
    {
        // Not passed in a custom one so use the Client.Timeout from the client instance
        // Edge case: it might be passed in exactly the same so that would change the timeout
        if (timeout == DefaultTimeout)
        {
            return Timeout;
        }

        if (timeout <= 0) return Timeout;

        return timeout;
    }

    string GetUriLabel(string typeName, Uri uri)
    {
        if (typeName == "Client")
        {
            typeName = uri.Host.ToLowerInvariant();
        }
        else
        {
            typeName = typeName.ToLowerInvariant();
        }

        string path = uri.AbsolutePath;

        if (path.Length <= 1)
        {
            return typeName.ToLowerInvariant();
        }

        if (path[^1] == '/')
        {
            path = path[..^1];
        }

        // Split path on '/' and take only the first three non-empty segments.
        int slashCount = 0;
        for (int i = 1; i < path.Length && slashCount < 3; i++)
        {
            if (path[i] == '/')
            {
                slashCount++;
                continue;
            }

            int end = path.IndexOf('/', i);

            if (end == -1) end = path.Length;

            typeName += $"/{path[i..end].ToLowerInvariant()}";

            i = end - 1;
        }

        // All rooted paths, like www.site.com/path/, /path2 and /longerPath3/ all gets same label: www.site.com
        if (slashCount == 0)
            return typeName.Split("/")[0];

        return typeName;
    }
}
