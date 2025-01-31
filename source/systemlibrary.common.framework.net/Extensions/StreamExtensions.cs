
namespace SystemLibrary.Common.Framework.Extensions;

/// <summary>
/// Extensions for streams like reading content as json directly into class through JsonAsync()
/// </summary>
public static class StreamExtensions
{
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
