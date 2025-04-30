using System.Text.Json;

namespace SystemLibrary.Common.Framework.App.Extensions;

public static class StringExtensions
{
    public static ClientResponse<T> Get<T>(this string url, ContentType contentType = ContentType.Auto, IDictionary<string, string> headers = null, int timeoutMilliseconds = Client.DefaultTimeout, JsonSerializerOptions jsonSerializerOptions = default, object payload = null, CancellationToken cancellationToken = default, Func<string, T> deserialize = null)
    {
        var client = new Client();

        return client.Get<T>(url, contentType, headers, timeoutMilliseconds, jsonSerializerOptions, payload, cancellationToken, deserialize);
    }

    public static ClientResponse<T> Post<T>(this string url, object data = null, ContentType contentType = ContentType.Auto, IDictionary<string, string> headers = null, int timeoutMilliseconds = Client.DefaultTimeout, JsonSerializerOptions jsonSerializerOptions = default, CancellationToken cancellationToken = default, Func<string, T> deserialize = null)
    {
        var client = new Client();

        return client.Post<T>(url, data, contentType, headers, timeoutMilliseconds, jsonSerializerOptions, cancellationToken, deserialize);
    }

    public static ClientResponse<T> Put<T>(this string url, object data, ContentType contentType = ContentType.Auto, IDictionary<string, string> headers = null, int timeoutMilliseconds = Client.DefaultTimeout, JsonSerializerOptions jsonSerializerOptions = default, CancellationToken cancellationToken = default, Func<string, T> deserialize = null)
    {
        var client = new Client();

        return client.Put<T>(url, data, contentType, headers, timeoutMilliseconds, jsonSerializerOptions, cancellationToken, deserialize);
    }

    public static ClientResponse<T> Delete<T>(this string url, object data, ContentType contentType = ContentType.Auto, IDictionary<string, string> headers = null, int timeoutMilliseconds = Client.DefaultTimeout, JsonSerializerOptions jsonSerializerOptions = default, CancellationToken cancellationToken = default, Func<string, T> deserialize = null)
    {
        var client = new Client();

        return client.Delete<T>(url, data, contentType, headers, timeoutMilliseconds, jsonSerializerOptions, cancellationToken, deserialize);
    }
}
