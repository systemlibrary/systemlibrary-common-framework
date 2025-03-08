using System.Text;

namespace SystemLibrary.Common.Framework.Extensions;

/// <summary>
/// Extension methods for ReadOnlySpan
/// </summary>
public static class ReadOnlySpanExtensions
{
    /// <summary>
    /// Checks if span is not null and length is larger than 0
    /// </summary>
    /// <example>
    /// <code>
    /// var textSpan = "Hello World".AsSpan();
    /// 
    /// var isSpan = textSpan.Is(); // True, span is not null
    /// </code>
    /// </example>
    /// <returns>True or false</returns>
    public static bool Is<T>(this ReadOnlySpan<T> span)
    {
        return span != null && span.Length != 0;
    }

    /// <summary>
    /// Checks if span is null or length is 0
    /// </summary>
    /// <example>
    /// <code>
    /// var textSpan = "Hello World".AsSpan();
    /// 
    /// var isSpan = textSpan.IsNot(); // False, as span is not null, and has text length &gt; 0
    /// </code>
    /// </example>
    /// <returns>True or false</returns>
    public static bool IsNot<T>(this ReadOnlySpan<T> span)
    {
        return span == null || span.Length == 0;
    }

    /// <summary>
    /// Convert a ReadOnlySpan of char to Base64
    /// </summary>
    /// <example>
    /// <code>
    /// var textSpan = "Hello World".AsSpan();
    /// 
    /// var base64 = textSpan.ToBase64();
    /// </code>
    /// </example>
    /// <returns>Base64 string or null if input was so</returns>
    public static string ToBase64(this ReadOnlySpan<char> span, Encoding encoding = default)
    {
        if (span == null) return default;

        Span<byte> bytes = new byte[span.Length];

        if (encoding == default)
        {
            Encoding.UTF8.GetBytes(span, bytes);

            return bytes.ToArray().ToBase64();
        }

        encoding.GetBytes(span, bytes);

        return bytes.ToArray().ToBase64();
    }

    /// <summary>
    /// Converts the ReadOnlySpan of Char to a byte array
    /// </summary>
    /// <example>
    /// <code>
    /// var textSpan = "Hello World".AsSpan();
    /// 
    /// var bytes = textSpan.GetBytes();
    /// </code>
    /// </example>
    /// <returns>Byte array or null if input was so</returns>
    public static byte[] GetBytes(this ReadOnlySpan<char> span, Encoding encoding = default)
    {
        if (span == null) return default;

        Span<byte> bytes = new byte[span.Length];

        if (encoding == default)
        {
            Encoding.UTF8.GetBytes(span, bytes);

            return bytes.ToArray();
        }

        encoding.GetBytes(span, bytes);

        return bytes.ToArray();
    }
}