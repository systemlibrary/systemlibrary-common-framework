using System;

namespace SystemLibrary.Common.Framework;

internal static class Obfuscate
{
    static int MaxChar = System.Convert.ToInt32(char.MaxValue);
    static int MinChar = System.Convert.ToInt32(char.MinValue);
    static int RangeSize = MaxChar - MinChar + 1;
    internal static string Convert(string text, int salt, bool deobfuscate)
    {
        if (salt <= 0)
            throw new Exception("Cannot obfuscate a string with a salt of 0 or less");

        if (text == null) return null;
        if (text == "") return "";

        if (deobfuscate)
            salt *= -1;

        Span<char> buffer = text.Length <= 127
            ? stackalloc char[text.Length]
            : new char[text.Length];

        for (int i = 0; i < text.Length; i++)
        {
            var shifted = text[i] + salt;

            if (shifted > MaxChar)
            {
                shifted -= RangeSize;
            }
            else if (shifted < MinChar)
            {
                shifted += RangeSize;
            }

            buffer[i] = (char)shifted;
        }

        return new string(buffer);
    }
}
