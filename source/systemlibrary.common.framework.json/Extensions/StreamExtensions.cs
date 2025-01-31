using System.Text.Json;

namespace SystemLibrary.Common.Framework.Extensions;

/// <summary>
/// Extensions for streams like reading content as json directly into class through JsonAsync()
/// </summary>
public static class StreamExtensions
{
    /// <summary>
    /// Reading a Stream async as JsonData converting it into a class T
    /// <para>Used for instance when you read the content of a HttpResponse and directly converting it into T instead of storing as string first</para>
    /// </summary>
    /// <example>
    /// <code>
    /// using (var contentStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
    ///    return await contentStream.JsonAsync&lt;T&gt;(jsonSerializerOptions).ConfigureAwait(false);
    /// </code>
    /// </example>
    /// <returns>T or default</returns>
    public static async Task<T> JsonAsync<T>(this Stream stream, JsonSerializerOptions options = null, CancellationToken cancellationToken = default)
    {
        if (stream == null) return default;

        options = JsonSerializerOptionsInstance.Current(options);

        return await JsonSerializer.DeserializeAsync<T>(stream, options, cancellationToken).ConfigureAwait(false);
    }
}
