using System.Collections.Concurrent;
using System.Reflection;
using System.Text;
using System.Web;

using Microsoft.AspNetCore.DataProtection;

using SystemLibrary.Common.Framework;
using SystemLibrary.Common.Framework.Extensions;

/// <summary>
/// This class contains extension methods for string
/// <para>StringExtensions exists in the global namespace</para>
/// </summary>
/// <example>
/// <code>
/// var result = "Hello world".Is()
/// // result is 'true'
/// </code>
/// <code>
/// var result = "".IsNot();
/// // result is 'true'
/// </code>
/// </example>
public static partial class StringExtensions
{
    internal static ConcurrentDictionary<int, MemberInfo[]> TypeEnumStaticMembers;
    static StringExtensions()
    {
        TypeEnumStaticMembers = new ConcurrentDictionary<int, MemberInfo[]>();
    }

    /// <summary>
    /// Returns data or the first fallback that exists
    /// <para>If all subsequent fallbacks are null, this returns ""</para>
    /// </summary>
    /// <example>
    /// <code class="language-csharp hljs">
    /// var text1 = null;
    /// var text2 = "";
    /// var text3 = " ";
    /// var text4 = "hello";
    /// var result = text1.OrFirstOf(text2, text3, text4);
    /// // result is "hello" as the others are blank/empty
    /// </code>
    /// </example>
    /// <returns>First non-null, non-empty and non-space string value, or empty string, never null.</returns>
    public static string OrFirstOf(this string text, params string[] fallbacks)
    {
        if (text.Is()) return text;

        if (fallbacks == null || fallbacks.Length == 0) return "";

        foreach (var fallback in fallbacks)
            if (fallback.Is())
                return fallback;

        return "";
    }

    /// <summary>
    /// Returns the domain part of the uri or blank, never null, includes ".com":
    /// <para>https://www.sub1.sub2.domain.com => domain.com</para>
    /// </summary>
    /// <inheritdoc cref="UriExtensions.GetPrimaryDomain"/>
    /// <example>
    /// <code class="language-csharp hljs">
    /// var result = new Uri('https://systemlibrary.com/image?q=90&amp;format=jpg').GetPrimaryDomain();
    /// // result is "systemlibrary.com"
    /// 
    /// var result = new Uri('https://systemlibrary.github.io/systemlibrary-common-framework/image?q=90&amp;format=jpg').GetPrimaryDomain();
    /// // result is "github.io"
    /// </code>
    /// </example>
    /// <returns>Primary domain or blank, never null</returns>
    public static string GetPrimaryDomain(this string url)
    {
        if (url == null || url.Length == 0) return "";

        if (url.Contains(" ")) return "";

        Uri uri = new Uri(url, UriKind.RelativeOrAbsolute);

        return uri.GetPrimaryDomain();
    }

    /// <summary>
    /// Returns a new string where all 'old values' are replaced with the 'newValue'
    /// <para>Does not throw on argument null</para>
    /// </summary>
    /// <example>
    /// <code>
    /// var text = "Hello world 12345";
    /// 
    /// var result = text.ReplaceAllWith("A", "Hello", "World", "123", "45");
    /// // result == A A AA, all mathing texts are replaces with the first param 'A'
    /// </code>
    /// </example>
    /// <returns>A new string with all replacements</returns>
    public static string ReplaceAllWith(this string text, string newValue, params string[] oldValues)
    {
        if (text == null) return text;

        var l = text.Length;

        if (l == 0) return text;

        if (newValue == null) return text;

        if (oldValues == null) return text;

        var nl = newValue.Length;

        var ol = oldValues.Length;
        if (ol > 1)
        {
            if (ol > 3 || l > 128 || nl > 64)
            {
                var sb = new StringBuilder(text);

                foreach (var oldValue in oldValues)
                    sb.Replace(oldValue, newValue);

                return sb.ToString();
            }
            else
            {
                foreach (var oldValue in oldValues)
                    text = text.Replace(oldValue, newValue);

                return text;
            }
        }

        return text.Replace(oldValues[0], newValue);
    }

    /// <summary>
    /// Convert a string value to Enum
    /// </summary>
    /// <typeparam name="T">T must be an Enum</typeparam>
    /// <param name="text">Value must match the Key or the 'EnumValueAttribute' or 'EnumTextAttribute' of a Key in the Enum (EnumValue is checked before EnumText), else this returns default of the Enum T</param>
    /// <example>
    /// <code class="language-csharp hljs">
    /// enum EnumColor
    /// {
    ///     None,
    ///     
    ///     [EnumText("White")]
    ///     [EnumValue("BlackAndWhite")]
    ///     Black,
    ///     
    ///     Pink
    /// }
    ///
    /// var value = "black".ToEnum&lt;EnumColor&gt;();
    /// // value is EnumColor.Black, case insensitive match directly in the Enum Key (or name if you prefer)
    /// 
    /// var value = "white".ToEnum&lt;EnumColor&gt;();
    /// // value is EnumColor.Black, case insensitive match in 'EnumText' attribute
    /// 
    /// var value = "blackAndWhite".ToEnum&lt;EnumColor&gt;();
    /// // value is EnumColor.Black, case insensitive match in 'EnumValue' attribute
    /// 
    /// var value = "brown".ToEnum&lt;EnumColor&gt;();
    /// // value is EnumColor.None, no match, returns first enum Key
    /// </code>
    /// </example>
    /// <returns>Returns first matching Key or default of the Enum</returns>
    public static T ToEnum<T>(this string text) where T : struct, IComparable, IFormattable, IConvertible
    {
        if (text == null) return default(T);

        return (T)text.ToEnum(typeof(T));
    }

    /// <summary>
    /// Convert a string to the Enum Type and cast as Object on returnal
    /// </summary>
    /// <returns>Returns first match or the first Key in the Enum</returns>
    public static object ToEnum(this string text, Type enumType)
    {
        object result;

        if (text != null && text.Length > 0 && char.IsDigit(text[0]))
        {
            if (Enum.TryParse(enumType, text, false, out result) || Enum.TryParse(enumType, text, true, out result))
            {
                var cacheKey = enumType.GetHashCode();

                var members = TypeEnumStaticMembers.Cache(cacheKey, () =>
                {
                    return enumType.GetMembers(BindingFlags.Public | BindingFlags.Static);
                });

                if (members?.Length > 0 && result != null)
                {
                    var n = result.ToString();
                    for (int i = 0; i < members.Length; i++)
                    {
                        if (members[i].Name == n)
                        {
                            return result;
                        }
                    }
                }
            }
        }
        else
        {
            if (Enum.TryParse(enumType, text, false, out result) || Enum.TryParse(enumType, text, true, out result))
            {
                return result;
            }
        }

        if (enumType.IsEnum)
        {
            var members = TypeEnumStaticMembers.Cache(enumType, () =>
            {
                return enumType.GetMembers(BindingFlags.Public | BindingFlags.Static);
            });

            if (members?.Length > 0)
            {
                text = text?.ToLower();

                var checkUnderscore = text?.Length > 1;

                foreach (var enumKey in members)
                {
                    if (enumKey.GetCustomAttribute(SystemType.EnumValueAttributeType) is EnumValueAttribute enumValueAttribute)
                    {
                        if (enumValueAttribute != null)
                        {
                            if (enumValueAttribute.Value is string svalue && svalue == text)
                            {
                                if (Enum.TryParse(enumType, enumKey.Name, out result))
                                    return result;
                            }
                            else if ((enumValueAttribute.Value + "").ToLower() == text)
                            {
                                if (Enum.TryParse(enumType, enumKey.Name, out result))
                                    return result;
                            }
                        }
                    }

                    if (enumKey.GetCustomAttribute(SystemType.EnumTextAttributeType) is EnumTextAttribute enumTextAttribute)
                    {
                        if (enumTextAttribute != null && enumTextAttribute.Text?.ToLower() == text)
                            if (Enum.TryParse(enumType, enumKey.Name, out result))
                                return result;
                    }

                    if (checkUnderscore && enumKey.Name[0] == '_')
                    {
                        if (text != null && enumKey.Name.EndsWith(text))
                        {
                            if (Enum.TryParse(enumType, enumKey.Name, out result))
                                return result;
                        }
                    }
                }
            }
        }

        if (result == null || text.IsNot())
            return Activator.CreateInstance(enumType);

        return result;
    }

    /// <summary>
    /// Returns true if text starts with any of the values, case sensitive
    /// </summary>
    /// <example>
    /// <code class="language-csharp hljs">
    /// var text = "hello world";
    /// 
    /// var result = text.StartsWithAny("", "abcdef", "hel");
    /// // result is true, due to the text begins with 'hel'
    /// </code>
    /// </example>
    /// <returns>True or false</returns>
    public static bool StartsWithAny(this string text, params string[] values)
    {
        if (text.IsNot()) return false;

        if (values == null || values.Length == 0) return false;

        var l = text.Length;
        for (int i = 0; i < values.Length; i++)
            if (values[i] != null && l >= values[i].Length && text.StartsWith(values[i], StringComparison.Ordinal))
                return true;

        return false;
    }

    /// <summary>
    /// Returns true if text starts with any of the values, case sensitive, using a String Comparison
    /// </summary>
    /// <example>
    /// <code class="language-csharp hljs">
    /// var text = "hello world";
    /// 
    /// var result = text.StartsWithAny(StringComparison.Ordinal, "", "abcdef", "hel");
    /// // result is true, due to the text begins with 'hel'
    /// </code>
    /// </example>
    /// <returns>True or false</returns>
    public static bool StartsWithAny(this string text, StringComparison comparison, params string[] values)
    {
        if (text.IsNot()) return false;

        if (values == null || values.Length == 0) return false;

        var l = text.Length;
        for (int i = 0; i < values.Length; i++)
            if (values[i] != null && l >= values[i].Length && text.StartsWith(values[i], comparison))
                return true;

        return false;
    }

    /// <summary>
    /// Returns true if text ends with any of the values, case sensitive
    /// </summary>
    /// <example>
    /// <code class="language-csharp hljs">
    /// var text = "hello world";
    /// var result = text.EndsWithAny("", "abdef", "rld");
    /// // result is true, because the last part of text ends with rld
    /// </code>
    /// </example>
    /// <returns>True or false</returns>
    public static bool EndsWithAny(this string text, params string[] values)
    {
        if (text == null || text.Length == 0) return false;

        if (values == null || values.Length == 0) return false;

        var l = text.Length;
        for (int i = 0; i < values.Length; i++)
            if (values[i] != null && l >= values[i].Length && text.EndsWith(values[i], StringComparison.Ordinal))
                return true;

        return false;
    }

    /// <summary>
    /// Returns true if text ends with any of the values, case sensitive, using StringComparison.Ordinal
    /// </summary>
    /// <example>
    /// <code class="language-csharp hljs">
    /// var text = "hello world";
    /// var result = text.EndsWithAny(StringComparison.Ordinal, "", "abdef", "rld");
    /// // result is true, because the last part of text ends with rld
    /// </code>
    /// </example>
    /// <returns>True or false</returns>
    public static bool EndsWithAny(this string text, StringComparison comparison, params string[] values)
    {
        if (text == null || text.Length == 0) return false;

        if (values == null || values.Length == 0) return false;

        var l = text.Length;
        for (int i = 0; i < values.Length; i++)
            if (values[i] != null && l >= values[i].Length && text.EndsWith(values[i], comparison))
                return true;

        return false;
    }

    /// <summary>
    /// Returns true if text equals any of the values, case sensitive
    /// </summary>
    /// <example>
    /// <code class="language-csharp hljs">
    /// var text = "hello world";
    /// var result = text.IsAny("hello", "world", "hello WORLD", "hello world");
    /// // result is true, as the last 'hello world' matches exactly
    /// </code>
    /// </example>
    /// <returns>True or false</returns>
    public static bool IsAny(this string text, params string[] values)
    {
        if (text.IsNot()) return false;

        if (values == null || values.Length == 0) return false;

        for (int i = 0; i < values.Length; i++)
            if (text == values[i])
                return true;

        return false;
    }

    /// <summary>
    /// Check if string is null, "" or " "
    /// </summary>
    /// <remarks>
    /// Example says "old way string.NullOrWhitespace", but it does not check multi spaces nor tabs, nor new lines, so not exactly the same
    /// </remarks>
    /// <example>
    /// <code class="language-csharp hljs">
    /// //Old way: string.IsNullOrWhiteSpace(text);
    /// 
    /// var text = " ";
    /// var result = text.IsNot();
    /// // result is true because a single space counts as "no text" in this function
    /// 
    /// var text = "  "; //2 spaces
    /// var result = text.IsNot();
    /// // result is false because two spaces counts as text in this function
    /// </code>
    /// </example>
    /// <returns>True or false</returns>
    public static bool IsNot(this string text, params string[] additionalNotValues)
    {
        if (text == null || text.Length == 0) return true;

        if (text.Length == 1 && text == " ") return true;

        if (additionalNotValues == null) return false;

        if (additionalNotValues.Any(value => text == value))
            return true;

        return false;
    }

    /// <summary>
    /// Check if string is not null, "" and " "
    /// </summary>
    /// <remarks>
    /// It does not check multiple spaces or new lines or tabs
    /// </remarks>
    /// <example>
    /// <code class="language-csharp hljs">
    /// // Old way: string.IsNullOrWhiteSpace(text);
    /// 
    /// var text = "hello world";
    /// var result = text.Is();
    /// // result is true because text is set to something, and not just "" or " "
    /// </code>
    /// </example>
    /// <returns>True or false</returns>
    public static bool Is(this string text)
    {
        if (text == null || text.Length == 0) return false;

        return !(text.Length == 1 && text == " ");
    }

    /// <summary>
    /// Check if string is not null, "" and " " and not any of the 'invalidTexts', else false
    /// <para>Case sensitive</para>
    /// </summary>
    /// <example>
    /// <code class="language-csharp hljs">
    /// var text = "hello world";
    /// var result = text.Is("hello");
    /// // result is true because text is set to something else than 'hello', and not just "" or " "
    /// 
    /// var result2 = text.Is("hello world");
    /// // result is false, because text equals to the invalid text passed in, which was 'hello world'
    /// </code>
    /// </example>
    /// <returns>True or false</returns>
    public static bool Is(this string text, params string[] invalidTexts)
    {
        if (text.IsNot())
            return false;

        if (invalidTexts == null || invalidTexts.Length == 0)
            return true;

        foreach (var word in invalidTexts)
            if (text == word)
                return false;

        return true;
    }

    /// <summary>
    /// Returns a new string with digits and letters only
    /// <para>Question marks, commas, exclamation marks, etc, are removed</para>
    /// </summary>
    /// <example>
    /// <code class="language-csharp hljs">
    /// var email = "support@system.library.com";
    /// var text = email.ToLetterAndDigits();
    /// 
    /// // text is "supportsystemlibrarycom"
    /// </code>
    /// </example>
    /// <returns>String with digits and characters, or blank if input is null or empty</returns>
    public static string ToLetterAndDigits(this string text)
    {
        if (text.IsNot()) return "";

        return new string(text.Where(x => char.IsLetterOrDigit(x)).ToArray());
    }

    /// <summary>
    /// Check if text contains any of the values, case sensitive
    /// </summary>
    /// <example>
    /// <code class="language-csharp hljs">
    /// var text = "hello";
    /// 
    /// var result = text.ContainsAny("123", "!", "lo");
    /// // result is true, because lo is part of the text
    /// </code>
    /// </example>
    /// <returns>True or false</returns>
    public static bool ContainsAny(this string text, params string[] values)
    {
        if (text.IsNot()) return false;

        if (values == null || values.Length == 0) return false;

        var l = text.Length;
        return values.Any(x => l >= x.Length && text.Contains(x, StringComparison.Ordinal));
    }

    /// <summary>
    /// Check if text contains any of the values, case sensitive, with a string comparison
    /// </summary>
    /// <example>
    /// <code class="language-csharp hljs">
    /// var text = "hello";
    /// 
    /// var result = text.ContainsAny(StringComparison.Ordinal, "123", "!", "lo");
    /// // result is true, because lo is part of the text
    /// </code>
    /// </example>
    /// <returns>True or false</returns>
    public static bool ContainsAny(this string text, StringComparison comparison, params string[] values)
    {
        if (text.IsNot()) return false;

        if (values == null || values.Length == 0) return false;

        var l = text.Length;

        return values.Any(x => l >= x.Length && text.Contains(x, comparison));
    }

    /// <summary>
    /// Trim the end of a string if it ends with any of the inputs
    /// <para>- It does not trim spaces, you must specify it as argument</para>
    /// <para>- Not recursive, returns after the first trimming</para>
    /// - Case sensitive
    /// </summary>
    /// <example>
    /// <code class="language-csharp hljs">
    /// var result = "abcd".TrimEnd(" ", "!", "c", "d");
    /// 
    /// // result is abc
    /// 
    /// var result = "abcd".TrimEnd(" ", "d", "bc");
    /// 
    /// // result is "abc" because it matches 'd' then returns, so 'bc' is never checked
    /// </code>
    /// </example>
    /// <returns>Returns the input string as is or without first value matched as ending</returns>
    public static string TrimEnd(this string text, params string[] values)
    {
        if (text.IsNot()) return text;

        if (values == null || values.Length == 0) return text;

        int start = 0;
        int valueLength;
        bool found = false;

        var textSpan = text.AsSpan();

        for (int i = 0; i < values.Length; i++)
        {
            valueLength = values[i].Length;
            start = text.Length - valueLength;

            for (int j = 0; j < valueLength; j++)
            {
                if (textSpan[start + j] != values[i][j])
                    break;

                if (j == valueLength - 1)
                    found = true;
            }

            if (found)
                break;
        }

        if (found)
            return textSpan.Slice(0, start).ToString();

        return textSpan.ToString();
    }

    /// <summary>
    /// Check if text ends ends with any characters from another string, case insensitive
    /// </summary>
    /// <param name="characters">Each character in this string will be checked</param>
    /// <example>
    /// <code class="language-csharp hljs">
    /// var text = "hello world";
    /// 
    /// var result = text.EndsWithAnyCharacter("abcdef");
    /// // result is true, because it ends with 'd'
    /// </code>
    /// </example>
    /// <returns>True or false</returns>
    public static bool EndsWithAnyCharacter(this string text, string characters)
    {
        if (text.IsNot()) return true;

        if (characters.IsNot()) return true;

        var textSpan = text.AsSpan();
        var span = characters.AsSpan();

        foreach (var character in span)
        {
            if (textSpan.EndsWith(character.ToString(), StringComparison.InvariantCultureIgnoreCase))
                return true;
        }

        return false;
    }

    /// <summary>
    /// Check if string is longer and returns a substring up to MaxLength
    /// </summary>
    /// <example>
    /// <code class="language-csharp hljs">
    /// var text = "hello world";
    /// var result = text.MaxLength(1);
    /// // result is "h" as it only can be 1 character long
    /// </code>
    /// </example> 
    /// <returns>Returns the string as it is if text is blank/null or shorter than maxLength, else returns a substring with a length of 'maxLength'. If maxLength is negative it returns ""</returns>
    public static string MaxLength(this string text, int maxLength)
    {
        if (text == null) return "";

        if (text.Length <= maxLength) return text;

        if (maxLength <= 0) return "";

        return new string(text.AsSpan(0, maxLength));
    }

    /// <summary>
    /// Darken or lighten a hex value by a factor
    /// <para>- pass a positive factor to darken</para>
    /// <para>- pass a negative factor to lighten</para>
    /// <para>- factor is a number between 0 and 1</para>
    /// - pass auto: true, to automatically check difference in the new value, and if the diff is too small (almost same color), the value is rather darkened instead of lightened, or ligtened instead of darkened
    /// </summary>
    /// <example>
    /// <code>
    /// var value = "#FFF";
    /// var newValue = value.HexDarkenOrLighten();
    /// // newValue is #4F4F4F
    /// </code>
    /// </example>
    /// <returns>A new hex darkened or lightend, or null/blank if input was so</returns>
    public static string HexDarkenOrLighten(this string hex, double factor = 0.31, bool auto = false)
    {
        if (hex.IsNot()) return hex;

        var hasHex = hex[0] == '#';
        if (hasHex)
            hex = hex.Substring(1);

        if (hex.Length != 6 && hex.Length != 3)
            throw new Exception("Hex is out of range, must be either 6 or 3, like: #FFF or #000000");

        int partLength = hex.Length == 6 ? 2 : 1;

        var color = new StringBuilder("");

        IEnumerable<string> SplitHex(string hex, int partLength)
        {
            for (var i = 0; i < hex.Length; i += partLength)
                yield return hex.Substring(i, Math.Min(partLength, hex.Length - i));
        }

        double colorValue;

        var minDiff = 55;
        foreach (var part in SplitHex(hex, partLength))
        {
            var number = Convert.ToInt32(part, 16);

            if (factor < 0)
            {
                colorValue = number - number * factor;

                if (auto && colorValue - number <= minDiff)
                {
                    if (colorValue <= 128)
                        colorValue = 255 - number;
                    else
                        colorValue -= number;
                }

                if (colorValue > 255)
                    colorValue -= 255;
            }
            else
            {
                colorValue = number * factor;

                if (auto && number - colorValue <= minDiff)
                {
                    if (colorValue <= 128)
                        colorValue = 255 - colorValue;
                    else
                        colorValue = number - colorValue;
                }
                if (colorValue > 255)
                    colorValue -= 255;

            }

            var temp = Convert.ToInt32(colorValue).ToString("X");

            if (temp.Length == 1)
                temp = "0" + temp;

            color.Append(temp);
        }

        if (hasHex)
            return "#" + color;

        return color.ToString();
    }

    /// <summary>
    /// Convert input to its Base64
    /// </summary>
    /// <remarks>
    /// If you dont need base64 format, .Obfuscating() method is faster if data is less than ~400KB
    /// </remarks>
    /// <example>
    /// <code>
    /// var value = "Hello world";
    /// var base64string = value.ToBase64();
    /// </code>
    /// </example>
    /// <returns>Base64 string, or null or blank if input was so</returns>
    public static string ToBase64(this string text, Encoding encoding = default)
    {
        return GetBytes(text, encoding).ToBase64();
    }

    /// <summary>
    /// Convert base64string back to a normal string
    /// </summary>
    /// <example>
    /// <code>
    /// var value = "Hello world";
    /// var base64string = value.ToBase64();
    /// var valueAgain = base64string.FromBase64();
    /// // value == valueAgain
    /// </code>
    /// </example>
    /// <returns>String or null/empty if input was so</returns>
    public static string FromBase64(this string base64String, Encoding encoding = default)
    {
        if (base64String == null) return null;

        if (encoding == default)
            encoding = Encoding.UTF8;

        return encoding.GetString(base64String.FromBase64AsBytes());
    }

    /// <summary>
    /// Returns the base64string input as a byte array
    /// </summary>
    /// <returns>String or null/empty if input was so</returns>
    public static byte[] FromBase64AsBytes(this string base64String)
    {
        if (base64String == null) return null;

        return Convert.FromBase64String(base64String);
    }

    /// <summary>
    /// Returns the byte representation of the text
    /// </summary>
    /// <example>
    /// <code>
    /// var value = "Hello world";
    /// var bytes = value.GetBytes();
    /// </code>
    /// </example>
    /// <returns>Byte array or null if input was null/empty</returns>
    public static byte[] GetBytes(this string text, Encoding encoding = default)
    {
        if (text == null) return null;

        if (encoding == null)
            return Encoding.UTF8.GetBytes(text);

        return encoding.GetBytes(text);
    }

    /// <summary>
    /// Obfuscate a string to a different string with a salt
    /// 
    /// <para>Throws exception if salt is &lt;= 0, salt should be in range from 1 to 65000</para>
    /// </summary>
    /// <remarks>
    /// Method .ToBase64() is faster if data is more than 400KB
    /// </remarks>
    /// <example>
    /// <code>
    /// var value = "Hello world";
    /// var obfuscatedText = value.Obfuscate();
    /// </code>
    /// </example>
    /// <returns>String or null/empty if input was so</returns>
    public static string Obfuscate(this string text, int salt = 11)
    {
        return SystemLibrary.Common.Framework.Obfuscate.Convert(text, salt, false);
    }

    /// <summary>
    /// Deobfuscate a string back to its readable state with a salt
    /// </summary>
    /// <remarks>
    /// Returns the text as it was before obfuscating, assuming you used the same salt value
    /// </remarks>
    /// <example>
    /// <code>
    /// var value = "Hello world";
    /// var obfuscatedText = value.Obfuscate();
    /// var deobfuscatedText = obfuscatedText.Deobfuscate();
    /// // value == deobfuscatedText
    /// </code>
    /// </example>
    /// <returns>String or null/empty if input was so</returns>
    public static string Deobfuscate(this string text, int salt = 11)
    {
        return SystemLibrary.Common.Framework.Obfuscate.Convert(text, salt, true);
    }

    /// <summary>
    /// Returns a MD5 hash version of the text input, resulting in a 47 character long text (including dashes).
    /// </summary>
    /// <remarks>
    /// If data is larger than ~200 bytes then .ToSha1Hash() is faster
    /// Md5 is not secure, there are rainbow tables, it's a 'one way shrinking obfuscater'
    /// </remarks>
    /// <example>
    /// <code>
    /// var value = "Hello world";
    /// var md5text = value.ToMD5Hash();
    /// </code>
    /// </example>
    /// <returns>Md5 hash string or null/empty if input was so</returns>
    public static string ToMD5Hash(this string text)
    {
        return Md5.Compute(text.GetBytes());
    }

    /// <summary>
    /// Returns a Sha1 hash version of the text input, resulting in a 59 character long text (including dashes). 
    /// </summary>
    /// <remarks>
    /// If data is smaller than ~200 bytes then .ToMD5Hash() is faster
    /// Sha1 is not secure, there are rainbow tables, it's a 'one way shrinking obfuscater'
    /// </remarks>
    /// <example>
    /// <code>
    /// var value = "Hello world";
    /// var sha1 = value.ToSha1Hash();
    /// </code>
    /// </example>
    /// <returns>Sha1 hash string or null/empty if input was so</returns>
    public static string ToSha1Hash(this string text)
    {
        return Sha1.Compute(text.GetBytes());
    }

    /// <summary>
    /// Returns a Sha256 hash version of the text input
    /// </summary>
    /// <example>
    /// <code>
    /// var value = "Hello world";
    /// var sha1 = value.ToSha1Hash();
    /// </code>
    /// </example>
    /// <returns>Sha256 hash string or null/empty if input was so</returns>
    public static string ToSha256Hash(this string text)
    {
        return Sha256.Compute(text.GetBytes());
    }

    /// <summary>
    /// Returns uri encoded version of a string, usually safe as a query parameter in a url for instance
    /// 
    /// <para>For instance: A 'space' becomes %20, and a '+' sign becomes %2B</para>
    /// </summary>
    /// <example>
    /// <code>
    /// var plain = "Hello world + ?";
    /// var coded = plain.UriEncode();
    /// //coded == "Hello%20world%20%2B%20%3F"
    /// </code>
    /// </example>
    public static string UriEncode(this string text)
    {
        if (text == null) return null;

        return Uri.EscapeDataString(text);
    }

    /// <summary>
    /// Returns uri decoded version of a uri encoded string
    /// <para>Example: For instance: %20 becomes 'space', and %2B becomes '+'</para>
    /// </summary>
    /// <example>
    /// <code>
    /// var coded = "Hello%20world%20%2B%20%3F";
    /// var plain = coded.UriDecode();
    /// //plain == "Hello world + ?"
    /// </code>
    /// </example>
    /// <returns>Uri decoded or null/blank if input was so</returns>
    public static string UriDecode(this string text)
    {
        if (text == null) return null;

        return Uri.UnescapeDataString(text);
    }

    /// <summary>
    /// Returns string as pascal cased, each character after a space or a dash is upper cased, while all others are forced lower cased
    /// </summary>
    /// <example>
    /// <code>
    /// var text = "abC deF";
    /// var result = text.ToPascalCase();
    /// //result == "Abc Def"
    /// </code>
    /// </example>
    /// <returns>string pascal cased or null/empty if input was so</returns>
    public static string ToPascalCase(this string text)
    {
        if (text.IsNot()) return text;

        if (char.IsLower(text[0]) && !text.Contains(" ") && !text.Contains("-"))
        {
            return char.ToUpper(text[0]) + text.Substring(1);
        }

        var length = text.Length;

        var sb = new StringBuilder(length);

        sb.Append(char.ToUpper(text[0]));

        var c = ' ';

        for (int i = 1; i < length; i++)
        {
            c = text[i];
            if (c == ' ')
            {
                i++;
                sb.Append(" " + char.ToUpper(text[i]));
            }
            else if (c == '-')
            {
                i++;
                sb.Append("-" + char.ToUpper(text[i]));
            }
            else
            {
                sb.Append(char.ToLower(c));
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// Returns string as camel cased
    /// 
    /// <para>Each word's first letter is upper case</para>
    /// <para>- words after a space or a dash</para>
    /// <para>- except the very first letter, it is lower cased</para>
    /// 
    /// All other letters are lower cased
    /// </summary>
    /// <example>
    /// <code>
    /// var text = "abC deF";
    /// var result = text.ToPascalCase();
    /// //result == "abc Def"
    /// </code>
    /// </example>
    /// <returns>camelCased version of input, or null/blank if input was so</returns>
    public static string toCamelCase(this string text)
    {
        if (text.IsNot()) return text;

        if (char.IsUpper(text[0]) && !text.Contains(" ") && !text.Contains("-"))
        {
            return char.ToLower(text[0]) + text.Substring(1);
        }

        var length = text.Length;

        var sb = new StringBuilder(length);

        sb.Append(char.ToLower(text[0]));

        var c = '0';

        for (int i = 1; i < length; i++)
        {
            c = text[i];
            if (c == ' ')
            {
                i++;
                sb.Append(" " + char.ToUpper(text[i]));
            }
            else if (c == '-')
            {
                i++;
                sb.Append("-" + char.ToUpper(text[i]));
            }
            else
            {
                sb.Append(char.ToLower(c));
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// Convert any uri to a application url, targetting files/folders inside your running app
    /// 
    /// <para>Assume app is hosted in C:/www/syslib/</para>
    /// 
    /// Examples: 
    /// http://www.systemlibrary.com/a returns C:\www\syslib\a 
    /// a returns C:/www/syslib/a
    /// /a returns C:/www/syslib/a
    /// a/ returns C:/www/syslib/a/
    /// /a/ returns C:/www/syslib/a/
    /// \a returns C:/www/syslib/a
    /// \a\b\ returns C:/www/syslib/a/b/
    /// 
    /// If input ends with slash, return value ends with slash
    /// </summary>
    /// <remarks>
    /// Always returns URL's with forward slashes
    /// <para>Server Root Path is read from CurrentDomain["ContentRootPath"] with fallback to AppContext.BaseDirectory.Parent. If a folder in the returning path is /bin/ it will navigate to the parent of such a folder then return</para>
    /// Remember that a URL is not a browser specific term, Uniform Resource Locator
    /// </remarks>
    /// <example>
    /// <code>
    /// var text = "/hello/world.txt";
    /// var result = text.ToPhysicalPath();
    /// // result == "C:/pub/www/hello/world.txt"
    /// 
    /// var text2 = "https://www.syslib.com/hello";
    /// var result2 = text2.ToPhysicalPath();
    /// // result2 == "C:/www/hello", no ending slash as text2 did not contain it
    /// </code>
    /// </example>
    /// <returns>Absolute application url based on input 'path', or null or empty if input was so</returns>
    public static string ToPhysicalPath(this string path)
    {
        if (path == null) return path;

        if (path.StartsWith("file:/")) return path;

        if (path.Length > 0 && path[0] == '~')
            path = path.Substring(1);

        if (path.Length > 1 && path.StartsWith(AppInstance.ContentRootPath))
            return path;

        var driveUriIndex = path.IndexOf(":/");
        if (driveUriIndex > 0 && driveUriIndex < 5)
        {
            if (path.Contains("\\"))
                return path.Replace("\\", "/");

            return path;
        }

        if (path.Contains(":\\", StringComparison.Ordinal)) return path.Replace("\\", "/").ToPhysicalPath();

        void ConvertWebPathToServerPath()
        {
            if (path.Contains("://", StringComparison.Ordinal))
            {
                if (path.Contains("?", StringComparison.Ordinal))
                    path = path.Split('?', 2)[0];

                var temp = new StringBuilder("");

                var parts = path.Substring(path.IndexOf("://", StringComparison.Ordinal) + 3).Split('/', StringSplitOptions.RemoveEmptyEntries);
                for (var i = 1; i < parts.Length; i++)
                {
                    temp.Append("/" + parts[i]);
                }

                if (path.EndsWith("/", StringComparison.Ordinal))
                    temp.Append("/");

                path = temp.ToString();
            }

            if (!path.StartsWith("/", StringComparison.Ordinal))
                path = "/" + path;

            path = path.Replace("/", "\\");
        }

        void ConvertToValidRelativeServerPath()
        {
            if (!path.StartsWith("\\", StringComparison.Ordinal))
                path = "\\" + path;
        }

        if (path.Contains("/", StringComparison.Ordinal))
        {
            ConvertWebPathToServerPath();
        }
        else
        {
            ConvertToValidRelativeServerPath();
        }
        return (AppInstance.ContentRootPath + path).Replace("\\", "/");
    }

    /// <summary>
    /// Encrypt data with a random generated IV
    /// <para>Key: If DataProtection has been setup</para>
    /// If key file is used, uses the filename
    /// <para>Else appName if set through SetApplicationName()</para>
    /// Else assembly name
    /// <para>Else no data protection usage: ABCDEFGHIJKLMNOPQRST123456789011</para>
    /// </summary>
    /// <param name="addIV">Add the generated IV to the output or not</param>
    /// <remarks>
    /// - Data protection key file is a XML file, file name starts with "key-"
    /// <para>- Built-in keys are always hashed before used as the Key</para>
    /// - Optionally add a random IV as the first 16 bytes of output. If you dont, the IV is 16 bytes of 0
    /// </remarks>
    /// <example>
    /// <code>
    /// var data = "Hello world";
    /// var encrypted = data.Encrypt();
    /// </code>
    /// </example>
    /// <returns>Encrypted base64 string with IV in first 16 bytes, or null/empty if input was so</returns>
    public static string Encrypt(this string data, bool addIV = true)
    {
        return Cryptation.Encrypt(data, CryptationKey.Current, null, addIV).ToBase64();
    }

    /// <summary>
    /// Encrypt data with a key and an optional IV
    /// <para>Key is null?: If DataProtection has been setup</para>
    /// If key file is used, uses the filename
    /// <para>Else appName if set through SetApplicationName()</para>
    /// Else assembly name
    /// <para>Else no data protection usage: ABCDEFGHIJKLMNOPQRST123456789011</para>
    /// Optional IV: If IV is not set then use either; random IV if addIV is true else 16 bytes of 0
    /// </summary>
    /// <param name="addIV">Add the generated IV to the output or not</param>
    /// <remarks>
    /// - Key must be 16 or 32 characters
    /// <para>- Optionally add IV to the fist 16 bytes of output, if not? IV is 16 bytes of 0</para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var key = "16 or 32 chars...";
    /// var data = "Hello world";
    /// var encrypted = data.Encrypt(key);
    /// </code>
    /// </example>
    /// <returns>Encrypted base64 string with IV in first 16 bytes if 'addIV' was true, or null/empty if input was so</returns>
    public static string Encrypt(this string data, string key, string IV = null, bool addIV = false)
    {
        return Encrypt(data, key.GetBytes() ?? CryptationKey.Current, IV.GetBytes(), addIV);
    }

    /// <summary>
    /// Encrypt data with a key and an optional IV
    /// <para>Key is null?: If DataProtection has been setup</para>
    /// If key file is used, uses the filename
    /// <para>Else appName if set through SetApplicationName()</para>
    /// Else assembly name
    /// <para>Else no data protection usage: ABCDEFGHIJKLMNOPQRST123456789011</para>
    /// Optional IV: If IV is not set then use either; random IV if addIV is true else 16 bytes of 0
    /// </summary>
    /// <param name="addIV">Add the generated IV to the output or not</param>
    /// <remarks>
    /// - Key must be 16 or 32 characters
    /// <para>- Optionally add IV to the fist 16 bytes of output, if not? IV is 16 bytes of 0</para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var key = "16 or 32 chars...".GetBytes();
    /// var data = "Hello world";
    /// var encrypted = data.Encrypt(key);
    /// </code>
    /// </example>
    /// <returns>Encrypted base64 string with IV in first 16 bytes if 'addIV' was true, or null/empty if input was so</returns>
    public static string Encrypt(this string data, byte[] key, byte[] IV = null, bool addIV = false)
    {
        if (key != null && key.Length != 16 && key.Length != 32)
            throw new Exception("Key length must be either 16 or 32");

        if (IV != null && IV.Length != 16)
            throw new Exception("AES must receive an IV of 16 characters length");

        return Cryptation.Encrypt(data, key ?? CryptationKey.Current, IV, addIV).ToBase64();
    }

    /// <summary>
    /// Returns the decrypted version of the cipher text
    /// </summary>
    /// <remarks>
    /// Must pass same arguments as you did when you invoked .Encrypt()
    /// <para>'addedIV' must be true if you set 'addIV' to 'true' during Encrypt()</para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var data = "Hello world";
    /// var encrypted = data.Encrypt();
    /// var decrypted = encrypted.Decrypt();
    /// </code>
    /// </example>
    /// <returns>Decrypted string or null/empty if input was so</returns>
    public static string Decrypt(this string cipherText, bool addedIV = true)
    {
        return Cryptation.Decrypt(cipherText, CryptationKey.Current, null, addedIV);
    }

    /// <summary>
    /// Returns the decrypted version of the cipher text
    /// </summary>
    /// <remarks>
    /// Must pass same arguments as you did when you invoked .Encrypt()
    /// <para>'addedIV' must be true if you set 'addIV' to 'true' during Encrypt()</para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var key = "16 or 32 chars...";
    /// var data = "Hello world";
    /// var encrypted = data.Encrypt(key);
    /// var decrypted = encrypted.Decrypt(key);
    /// </code>
    /// </example>
    /// <returns>Decrypted string or null/empty if input was so</returns>
    public static string Decrypt(this string cipherText, string key, string IV = null, bool addedIV = false)
    {
        return Decrypt(cipherText, key.GetBytes() ?? CryptationKey.Current, IV.GetBytes(), addedIV);
    }

    /// <summary>
    /// Returns the decrypted version of the cipher text
    /// </summary>
    /// <remarks>
    /// Must pass same arguments as you did when you invoked .Encrypt()
    /// <para>'addedIV' must be true if you set 'addIV' to 'true' during Encrypt()</para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var key = "16 or 32 chars...".GetBytes();
    /// var data = "Hello world";
    /// var encrypted = data.Encrypt(key);
    /// var decrypted = encrypted.Decrypt(key);
    /// </code>
    /// </example>
    /// <returns>Decrypted string or null/empty if input was so</returns>
    public static string Decrypt(this string cipherText, byte[] key, byte[] IV = null, bool addedIV = false)
    {
        if (key != null && key.Length != 16 && key.Length != 32)
            throw new Exception("Key length must be either 16 or 32");

        if (IV != null && IV.Length != 16)
            throw new Exception("AES must receive an IV of 16 characters length");

        return Cryptation.Decrypt(cipherText, key ?? CryptationKey.Current, IV, addedIV);
    }

    /// <summary>
    /// Translate unicode code points to characters
    /// <para>Example: HellU+00F8 is converted into Hellø (NOR char oslash;)</para>
    /// <para>and</para>
    /// <para>Hell\u00F8 is converted also into Hellø (NOR char oslash;)</para>
    /// </summary>
    /// <returns>Translated text</returns>
    public static string TranslateUnicodeCodepoints(this string data)
    {
        if (data.IsNot()) return data;

        if (data.Length < 4) return data;

        var sb = new StringBuilder(data);
        foreach (System.Text.RegularExpressions.Match m in new System.Text.RegularExpressions.Regex(@"\\u(\w{4})").Matches(data))
        {
            sb = sb.Replace(m.Value, ((char)(int.Parse(m.Value.Substring(2), System.Globalization.NumberStyles.AllowHexSpecifier))).ToString());
        }
        foreach (System.Text.RegularExpressions.Match m in new System.Text.RegularExpressions.Regex(@"[Uu][+](\w{4})").Matches(data))
        {
            sb = sb.Replace(m.Value, ((char)(int.Parse(m.Value.Substring(2), System.Globalization.NumberStyles.AllowHexSpecifier))).ToString());
        }
        return sb.ToString();
    }

    // CREDS TO: https://learn.microsoft.com/en-us/answers/questions/226531/c-best-method-to-reduce-size-of-large-string-data.html
    /// <summary>
    /// Compress the input data and return
    /// </summary>
    /// <remarks>
    /// Returned value might be larger if the input is only a character or two.
    /// </remarks>
    /// <example>
    /// <code>
    /// var data = "Hello world";
    /// var compressed = data.Compress();
    /// </code>
    /// </example>
    /// <returns>Compressed version of input as Base64, or null/empty if input was so</returns>
    public static string Compress(this string data, Encoding encoding = null)
    {
        if (data == null || data == "") return data;

        var bytes = data.GetBytes(encoding);

        return bytes.Compress();
    }

    /// <summary>
    /// Decompress compressed data and return the result
    /// </summary>
    /// <example>
    /// <code>
    /// var data = "Hello world";
    /// var compressed = data.Compress();
    /// var decompressed = compressed.Decompress();
    /// Assert.IsTrue(data == decompressed);
    /// </code>
    /// </example>
    /// <param name="compressedData">A base64 compressed version of data</param>
    /// <returns>Decompressed version of input, or null/empty if input was so</returns>
    public static string Decompress(this string compressedData, Encoding encoding = null)
    {
        if (compressedData.IsNot()) return compressedData;

        var bytes = compressedData.FromBase64AsBytes();

        return bytes.Decompress(encoding);
    }

    /// <summary>
    /// Html encode input and return the result
    /// <para>Example: > becomes ampersandGT;</para>
    /// </summary>
    /// <example>
    /// <code>
    /// var html = ">";
    /// var encoded = html.HtmlEncode();
    /// // result contains ampersand gt; 
    /// </code>
    /// </example>
    /// <returns>HtmlEncoded version of input, if input is null/empty it returns null/empty</returns>
    public static string HtmlEncode(this string text)
    {
        if (text.Is())
            return HttpUtility.HtmlEncode(text);

        return text;
    }

    /// <summary>
    /// Html decode input and return the result
    /// <para>Example: ampersandGT; becomes ></para>
    /// </summary>
    /// <example>
    /// <code>
    /// var html = "ampersand gt;";
    /// var encoded = html.HtmlDecode();
    /// // result equals >
    /// // assume ampersand is the character, Microsofts Docfx has a ton of limits and their tactic to go with XML is to vomit of
    /// </code>
    /// </example>
    /// <returns>HtmlDecoded version of input, if input is null/empty it returns null/empty</returns>
    public static string HtmlDecode(this string htmlEncodedText)
    {
        if (htmlEncodedText.Is())
            return HttpUtility.HtmlDecode(htmlEncodedText);

        return htmlEncodedText;
    }

    /// <summary>
    /// Convert input to Utf8 BOM
    /// </summary>
    /// <returns>A new string Utf8BOM encoded</returns>
    public static string ToUtf8BOM(this string text)
    {
        if (text.IsNot()) return text;

        var first = (text[0] + "").GetBytes();

        if (first?.Length == 3)
        {
            if (first[0] == 239 && first[1] == 187 && first[2] == 191)
            {
                return text;
            }
        }

        var utf8BOM = new UTF8Encoding(true, false);
        var lvBOM = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
        var bytes = utf8BOM.GetBytes(lvBOM + text);

        return Encoding.UTF8.GetString(bytes);
    }

    /// <summary>
    /// Convert string to integer
    /// <para>Returns 0 on empty/null strings</para>
    /// Throws if string is not a number
    /// </summary>
    /// <returns>Converted string as Integer</returns>
    public static int ToInt(this string number)
    {
        if (number.IsNot()) return 0;

        return Convert.ToInt32(number);
    }

    /// <summary>
    /// Convert string to Int64
    /// <para>Returns 0 on empty/null strings</para>
    /// Throws if string is not a number
    /// </summary>
    /// <returns>Converted string as Int64</returns>
    public static Int64 ToInt64(this string number)
    {
        if (number.IsNot()) return 0;

        return Convert.ToInt64(number);
    }
    /// <summary>
    /// Encrypt data which can be Decrypted through 'DecryptUsingKeyRing'
    /// <para>Uses the Data Protection API from Microsoft, which uses your setup of the AddDataProtection() services.</para>
    /// </summary>
    /// <example>
    /// var text = "hello world";
    /// var cipherText = text.EncryptUsingKeyRing();
    /// </example>
    public static string EncryptUsingKeyRing(this string data)
    {
        return KeyRingProtectorInstance.Current.Protect(data);
    }

    /// <summary>
    /// Decrypt data that was Encrypted through 'EncryptUsingKeyRing'
    /// <para>Uses the Data Protection API from Microsoft, which uses your setup of the AddDataProtection() services.</para>
    /// </summary>
    /// <example>
    /// var text = "hello world";
    /// var cipherText = text.EncryptUsingKeyRing();
    /// var textAgain = cipherText.DecryptUsingKeyRing();
    /// </example>
    public static string DecryptUsingKeyRing(this string data)
    {
        return KeyRingProtectorInstance.Current.Unprotect(data);
    }

    /// <summary>
    /// Checks if input is a file path, either relative or absolute, either web or operative system path
    /// </summary>
    /// <remarks>
    /// Does not throw
    /// <para>Returns true if input is longer than 4, and contains a 'file extension' of 1 to 6 characters, else false</para>
    /// <para>Returns true if input contains /public/, /static/, /images/ or 'assets/' as we assume a file is asked for</para>
    /// <para>Supports input as relative, absolute, and with url query params</para>
    /// <para>Returns false if input exceeds 4096 characters</para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var hello = "world";
    /// var isFile = hello.IsFile(); // false
    /// 
    /// var file = "/image/redcar1.jpg?qualit=80";
    /// var isFile = file.IsFile(); // true
    /// 
    /// var file2 = "/assets/bluecar";
    /// var isFile = file2.IsFile(); // true, assumes any "assets/" request is a file
    /// </code>
    /// </example>
    /// <returns>True or false</returns>
    public static bool IsFile(this string path)
    {
        if (path == null || path.Length <= 3 || path.Length > 4096)
            return false;

        if (path.EndsWith("/")) return false;

        if (path.IndexOfAny(Path.GetInvalidPathChars()) != -1) return false;

        if (path.Contains("<")) return false;

        bool HasAssetPath() =>
            path.Contains("/public/", StringComparison.OrdinalIgnoreCase) ||
            path.Contains("/images/", StringComparison.OrdinalIgnoreCase) ||
            path.Contains("/static/", StringComparison.OrdinalIgnoreCase) ||
            path.Contains("assets/", StringComparison.OrdinalIgnoreCase);

        var extensionIndex = path.LastIndexOf('.');

        if (extensionIndex == -1) return HasAssetPath();

        var queryIndex = path.IndexOf('?');

        if (queryIndex == -1)
        {
            if (extensionIndex == path.Length - 1) return HasAssetPath();

            if (path.LastIndexOf('/') > extensionIndex) return HasAssetPath();

            return extensionIndex >= path.Length - 7 || HasAssetPath(); // .config
        }

        if (extensionIndex > queryIndex)
        {
            var temp = path.Substring(0, queryIndex);

            return temp.LastIndexOf('.') >= temp.Length - 7 || HasAssetPath(); // .config
        }

        if (path[queryIndex - 1] == '/') return false;

        return queryIndex - 7 <= extensionIndex || HasAssetPath();
    }

    internal static string ToOsFriendlyPath(this string path)
    {
        if (path.IsNot()) return path;

        if (path.IndexOf('\\') > -1)
        {
            path = path.Replace("\\", "/");
        }

        return path.TrimEnd('/');
    }
}
