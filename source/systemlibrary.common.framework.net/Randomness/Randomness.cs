using System.Text;

namespace SystemLibrary.Common.Framework;

/// <summary>
/// Shortcut for generating random values using System.Random
/// </summary>
public static class Randomness
{
    static char[] Chars;

    static int CharsLength;

    static Randomness()
    {
        Chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();
        CharsLength = Chars.Length;
    }

    /// <summary>
    /// Generate a random Integer
    /// </summary>
    /// <returns>Integer >= 0</returns>
    public static int Int(int maxValue = int.MaxValue)
    {
        return Random.Shared.Next(0, maxValue);
    }

    /// <summary>
    /// Generate a random Integer
    /// </summary>
    /// <returns>Integer >= 0</returns>
    public static int Int(int minValue, int maxValue)
    {
        return Random.Shared.Next(minValue, maxValue);
    }

    /// <summary>
    /// Generate a random Double
    /// </summary>
    /// <returns>Integer >= 0</returns>
    public static double Double(double minValue = 1, double maxValue = 9999)
    {
        double randomValue = Random.Shared.NextDouble();

        return minValue + (randomValue * (maxValue - minValue));
    }

    /// <summary>
    /// Generate a random Byte array
    /// </summary>
    /// <returns>Byte array filled with random int's</returns>
    public static byte[] Bytes(int length = 16)
    {
        var bytes = new byte[length];

        Random.Shared.NextBytes(bytes);

        return bytes;
    }

    /// <summary>
    /// Generate a random string
    /// </summary>
    /// <returns>Returns a string of length</returns>
    public static string String(int length = 6)
    {
        var result = new StringBuilder(length);
        for (int i = 0; i < length; i++)
        {
            result.Append(Chars[Random.Shared.Next(CharsLength)]);
        }
        return result.ToString();
    }
}
