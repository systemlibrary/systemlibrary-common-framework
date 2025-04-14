using System.Text;

namespace SystemLibrary.Common.Framework.Extensions;

/// <summary>
/// String Builder extensions
/// </summary>
public static class StringBuilderExtensions
{
    /// <summary>
    /// Check if stringbuilder is not null and has content
    /// </summary>
    /// <returns>True or false</returns>
    public static bool Is(this StringBuilder stringBuilder)
    {
        return stringBuilder != null && stringBuilder.Length != 0;
    }

    /// <summary>
    /// Check if stringbuilder is null or has no content
    /// </summary>
    /// <returns>True or false</returns>
    public static bool IsNot(this StringBuilder stringBuilder)
    {
        return stringBuilder == null || stringBuilder.Length == 0;
    }

    /// <summary>
    /// Check if stringbuilder ends with a certain text
    /// </summary>
    /// <returns>True or false</returns>
    public static bool EndsWith(this StringBuilder stringBuilder, string ending, bool caseInsensitive = false)
    {
        if (stringBuilder == null || stringBuilder.Length == 0) return false;

        if (ending == null || ending == "") return false;

        var endingLength = ending.Length;

        if (endingLength > stringBuilder.Length) return false;

        var startIndex = stringBuilder.Length - endingLength;
        var endIndex = startIndex + endingLength;
        var j = 0;

        if (!caseInsensitive)
        {
            for (var i = startIndex; i < endIndex; i++)
            {
                if (stringBuilder[i] != ending[j]) return false;
                j++;
            }
        }
        else
        {
            for (var i = startIndex; i < endIndex; i++)
            {
                if (char.ToLowerInvariant(stringBuilder[i]) != char.ToLowerInvariant(ending[j])) return false;
                j++;
            }
        }
        return true;
    }

    /// <summary>
    /// Check if stringbuilder ends with certain texts, if so, the first ending match was removed
    /// </summary>
    /// <returns>True if text was removed, else false</returns>
    public static bool TrimEnd(this StringBuilder stringBuilder, params string[] values)
    {
        if (values == null || values.Length == 0) return false;

        for (int i = 0; i < values.Length; i++)
        {
            var value = values[i];
            if (stringBuilder.EndsWith(value, false))
            {
                var length = value.Length;

                stringBuilder.Remove(stringBuilder.Length - length, length);

                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Returns the index of the text within the StringBuilder
    /// </summary>        
    /// <param name="text">The string to find</param>
    /// <param name="start">The starting index.</param>
    /// <param name="ignoreCase">if set to <c>true</c> it will ignore case</param>
    /// <returns>Int or -1 if not found</returns>
    public static int IndexOf(this StringBuilder stringBuilder, string text, bool ignoreCase = false, int start = 0)
    {
        //Creds: https://stackoverflow.com/questions/1359948/why-doesnt-stringbuilder-have-indexof-method
        if (stringBuilder == null) return -1;

        if (text == null) return -1;

        int index;
        int length = text.Length;

        if (length > stringBuilder.Length) return -1;

        int maxSearchLength = (stringBuilder.Length - length) + 1;

        if (ignoreCase)
        {
            var textLowered = text.ToLower();
            for (int i = start; i < maxSearchLength; ++i)
            {
                if (Char.ToLower(stringBuilder[i]) == textLowered[0])
                {
                    index = 1;
                    while ((index < length) && (Char.ToLower(stringBuilder[i + index]) == textLowered[index]))
                        ++index;

                    if (index == length)
                        return i;
                }
            }

            return -1;
        }

        for (int i = start; i < maxSearchLength; ++i)
        {
            if (stringBuilder[i] == text[0])
            {
                index = 1;
                while ((index < length) && (stringBuilder[i + index] == text[index]))
                    ++index;

                if (index == length)
                    return i;
            }
        }

        return -1;
    }

    static Dictionary<string, string> HtmlEncodeReplacements = new Dictionary<string, string>
    {
        { "\"", "&quot;" },
        { "'", "&apos;" },
    };

    /// <summary>
    /// Replaces " with &quot; and single quote with &apos; within the StringBuilder
    /// <para>Throws on null argument</para>
    /// </summary>
    public static StringBuilder HtmlEncodeQuotes(this StringBuilder html, Dictionary<string, string> additionalReplacements = null)
    {
        foreach (var replacement in HtmlEncodeReplacements)
            html.Replace(replacement.Key, replacement.Value);

        if (additionalReplacements != null)
        {
            foreach (var replacement in additionalReplacements)
                html.Replace(replacement.Key, replacement.Value);
        }

        return html;
    }

    static Dictionary<string, string> HtmlDecodeReplacements = new Dictionary<string, string>
    {
        { "&quot;", "\"" },
        { "&apos;", "'" }
    };

    /// <summary>
    /// Replaces &quot; with " and &apos; with a single quote within the StringBuilder
    /// <para>Throws on null argument</para>
    /// </summary>
    public static StringBuilder HtmlDecodeQuotes(this StringBuilder html, Dictionary<string, string> additionalReplacements = null)
    {
        foreach (var replacement in HtmlDecodeReplacements)
            html.Replace(replacement.Key, replacement.Value);

        if (additionalReplacements != null)
        {
            foreach (var replacement in additionalReplacements)
                html.Replace(replacement.Key, replacement.Value);
        }
        return html;
    }

    /// <summary>
    /// Reduce the string builder to a fixed max length
    /// <para>Does nothing if stringbuilder is null or less than or equal to the max length specified</para>
    /// </summary>
    /// <remarks>
    /// Does not throw
    /// Returns null if null was input
    /// </remarks>
    /// <example>
    /// <code>
    /// var sb = new StringBuilder("hello world");
    /// sb.MaxLength(1);
    /// var text = sb.ToString();
    /// // text == "h"
    /// </code>
    /// </example>
    /// <param name="maxLength">Max amount of characters to keep</param>
    public static StringBuilder MaxLength(this StringBuilder stringBuilder, int maxLength)
    {
        if (stringBuilder == null || stringBuilder.Length <= maxLength) return stringBuilder;

        stringBuilder.Length = maxLength;

        return stringBuilder;
    }

    /// <summary>
    /// Returns a short string representation of the data through hashing and sample hashing
    /// <para>If inputs length is less than or equal to 4 returns input as is</para>
    /// <para>If inputs length exceeds 256 length, we hash only the start, middle and end of the string, avoidiing a lot of CPU for the risk of more collisions</para>
    /// </summary>
    public static string GetCompressedKey(this StringBuilder input)
    {
        if (input == null) return "";

        var l = input.Length;

        if (l <= 4) return input.ToString();

        var count = 32;
        if (l < 64)
        {
            count = l / 2;
        }
        int hash = 0;
        var iEnd = l - 1;
        for (var i = 0; i < count; i++)
        {
            hash += (input[i] * 11) + (input[iEnd - i] * 11);
        }

        if (l <= 66)
            return hash.ToString() + l;

        hash += (input[l / 2] * 11);
        hash += (input[l / 3] * 11);
        hash += (input[l / 4] * 11);

        return hash.ToString() + l;
    }
}
