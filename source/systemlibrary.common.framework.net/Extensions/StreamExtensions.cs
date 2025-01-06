using System.Text.Json;

namespace SystemLibrary.Common.Framework.Extensions;

/// <summary>
/// Extensions for streams like reading content as json directly into class through JsonAsync()
/// </summary>
public static class StreamExtensions
{
    /// <summary>
    /// Reading a Stream async as JsonData converting it into a class T
    /// 
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

    /// <summary>
    /// Reads a stream and hashing its content as MD5
    /// </summary>
    /// <remarks>
    /// If data is larger than ~200 bytes then .ToSha1Hash() is faster
    /// </remarks>
    /// <example>
    /// <code>
    /// var fileStream = new FileStream(@"C:\file.txt", FileMode.Open);
    /// var hash = fileStream.ToMD5HashAsync().Result;
    /// </code>
    /// </example>
    /// <returns>Md5 hash or null if unreadable stream</returns>
    public static async Task<string> ToMD5HashAsync(this Stream stream)
    {
        return await Md5.ComputeAsync(stream).ConfigureAwait(false);
    }

    /// <summary>
    /// Reads a stream and hashing its content as MD5
    /// </summary>
    /// <remarks>
    /// If data is larger than ~200 bytes then .ToSha1Hash() is faster
    /// </remarks>
    /// <example>
    /// <code>
    /// var fileStream = new FileStream(@"C:\file.txt", FileMode.Open);
    /// var hash = fileStream.ToMD5Hash();
    /// </code>
    /// </example>
    /// <returns>Md5 hash or null if unreadable stream</returns>
    public static string ToMD5Hash(this Stream stream)
    {
        return Md5.Compute(stream);
    }

    /// <summary>
    /// Reads a stream and hashing its content as Sha1
    /// </summary>
    /// <remarks>
    /// If data is smaller than ~200 bytes then .ToMD5Hash() is faster
    /// </remarks>
    /// <example>
    /// <code>
    /// var fileStream = new FileStream(@"C:\file.txt", FileMode.Open);
    /// var hash = fileStream.ToSha1Hash();
    /// </code>
    /// </example>
    /// <returns>Sha1 hash or null if unreadable stream</returns>
    public static string ToSha1Hash(this Stream stream)
    {
        return Sha1.Compute(stream);
    }

    /// <summary>
    /// Reads a stream and hashing its content as Sha1
    /// </summary>
    /// <remarks>
    /// If data is smaller than ~200 bytes then .ToMD5Hash() is faster
    /// </remarks>
    /// <example>
    /// <code>
    /// var fileStream = new FileStream(@"C:\file.txt", FileMode.Open);
    /// var hash = fileStream.ToSha1HashAsync().Result;
    /// </code>
    /// </example>
    /// <returns>Sha1 hash or null if unreadable stream</returns>
    public static async Task<string> ToSha1HashAsync(this Stream stream)
    {
        return await Sha1.ComputeAsync(stream).ConfigureAwait(false);
    }

    /// <summary>
    /// Reads a stream and hashing its content as Sha256
    /// </summary>
    /// <example>
    /// <code>
    /// var fileStream = new FileStream(@"C:\file.txt", FileMode.Open);
    /// var hash = fileStream.ToSha256Hash();
    /// </code>
    /// </example>
    /// <returns>Sha256 hash or null if unreadable stream</returns>
    public static string ToSha256Hash(this Stream stream)
    {
        return Sha256.Compute(stream);
    }

    /// <summary>
    /// Reads a stream and hashing its content as Sha256
    /// </summary>
    /// <example>
    /// <code>
    /// var fileStream = new FileStream(@"C:\file.txt", FileMode.Open);
    /// var hash = fileStream.ToSha256HashAsync().Result;
    /// </code>
    /// </example>
    /// <returns>Sha256 hash or null if unreadable stream</returns>
    public static async Task<string> ToSha256HashAsync(this Stream stream)
    {
        return await Sha256.ComputeAsync(stream).ConfigureAwait(false);
    }
}
