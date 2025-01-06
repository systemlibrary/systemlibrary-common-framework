using System.Security.Cryptography;

namespace SystemLibrary.Common.Framework;

internal static class Sha1
{
    internal static string Compute(byte[] bytes)
    {
        if (bytes == null) return null;

        if (bytes.Length == 0) return "";

        using (var hasher = SHA1.Create())
            return BitConverter.ToString(hasher.ComputeHash(bytes));
    }

    internal static string Compute(Stream stream)
    {
        if (stream == null) return null;

        if (!stream.CanRead) return null;

        using (var hasher = SHA1.Create())
            return BitConverter.ToString(hasher.ComputeHash(stream));
    }

    internal static async Task<string> ComputeAsync(Stream stream)
    {
        if (stream == null) return null;

        if (!stream.CanRead) return null;

        using (var hasher = SHA1.Create())
            return BitConverter.ToString(await hasher.ComputeHashAsync(stream).ConfigureAwait(false));
    }
}
