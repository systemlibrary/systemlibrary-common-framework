using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace SystemLibrary.Common.Framework.Extensions;

/// <summary>
/// Byte array extensions
/// </summary>
public static class ByteArrayExtensions
{
    /// <summary>
    /// Return a base64 string of the bytes
    /// </summary>
    /// <remarks>
    /// If you dont need base64 format, .Obfuscating() method is faster if data is less than ~400KB
    /// </remarks>
    /// <example>
    /// <code>
    /// var bytes = "hello world".GetBytes();
    /// var base64string = bytes.ToBase64();
    /// </code>
    /// </example>
    /// <returns>Returns Base64 string or null or empty if input was so</returns>
    public static string ToBase64(this byte[] bytes)
    {
        if (bytes == null) return null;

        return Convert.ToBase64String(bytes);
    }

    /// <summary>
    /// Return a text representation of the byte array
    /// </summary>
    /// <example>
    /// <code>
    /// var bytes = "hello world".GetBytes();
    /// var text = bytes.ToText();  //text == hello world
    /// </code>
    /// </example>
    /// <returns>Text, or null or empty if input was null or empty</returns>
    public static string ToText(this byte[] bytes, Encoding encoding = default)
    {
        if (bytes == null) return null;

        if (bytes.Length == 0) return "";

        if (encoding == null)
            return Encoding.UTF8.GetString(bytes);

        return encoding.GetString(bytes);
    }

    /// <summary>
    /// Returns a sha1 hash string of the bytes
    /// </summary>
    /// <remarks>
    /// If data is less than ~200 bytes then .ToMD5Hash() is faster
    /// </remarks>
    /// <example>
    /// <code>
    /// var bytes = "hello world".GetBytes();
    /// var sha1string = bytes.ToSha1Hash();
    /// </code>
    /// </example>
    /// <returns>A Sha1 hash or null or empty if input is null or empty</returns>
    public static string ToSha1Hash(this byte[] bytes)
    {
        if (bytes == null) return null;

        if (bytes.Length == 0) return "";

        return Sha1.Compute(bytes);
    }

    /// <summary>
    /// Returns a sha 256 hash string of the bytes
    /// </summary>
    /// <remarks>
    /// If input is null or empty it returns null or empty string
    /// </remarks>
    /// <example>
    /// <code>
    /// var bytes = "hello world".GetBytes();
    /// var sha256string = bytes.ToSha256Hash();
    /// </code>
    /// </example>
    /// <returns>A Sha256 hash, or null or empty if input was so</returns>
    public static string ToSha256Hash(this byte[] bytes)
    {
        if (bytes == null) return null;

        if (bytes.Length == 0) return "";

        return Sha256.Compute(bytes);
    }

    /// <summary>
    /// Returns a hash string of the bytes
    /// </summary>
    /// <remarks>
    /// If data is larger than ~200 bytes then .ToSha1Hash() is faster
    /// </remarks>
    /// <example>
    /// <code>
    /// var bytes = "hello world".GetBytes();
    /// var md5string = bytes.ToMD5Hash();
    /// </code>
    /// </example>
    /// <returns>Md5 hash or null or empty if input was so</returns>
    public static string ToMD5Hash(this byte[] bytes)
    {
        if (bytes == null) return null;

        if (bytes.Length == 0) return "";

        return Md5.Compute(bytes);
    }

    public static string Compress(this byte[] bytes)
    {
        if (bytes == null) return null;

        if (bytes.Length == 0) return "";

        using (var input = new MemoryStream(bytes))
        {
            using (var output = new MemoryStream())
            {
                using (var stream = new GZipStream(output, CompressionLevel.SmallestSize))
                {
                    input.CopyToAsync(stream);
                }

                return output.ToArray().ToBase64();
            }
        }
    }

    public static string Decompress(this byte[] bytes, Encoding encoding = null)
    {
        if (bytes == null) return null;
        if (bytes.Length == 0) return "";

        using (var output = new MemoryStream())
        {
            using (var input = new MemoryStream(bytes))
            {
                using (var stream = new GZipStream(input, CompressionMode.Decompress))
                {
                    stream.CopyToAsync(output);
                }
            }

            if (encoding == null)
                return Encoding.UTF8.GetString(output.ToArray());

            return encoding.GetString(output.ToArray());
        }
    }
}
