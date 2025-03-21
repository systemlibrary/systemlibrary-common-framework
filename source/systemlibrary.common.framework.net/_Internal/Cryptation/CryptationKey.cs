using System.Text;

namespace SystemLibrary.Common.Framework;

internal static partial class CryptationKey
{
    internal static readonly string _KeyFileFullName;

    internal static string KeyStart;

    internal static byte[] Current = Encoding.UTF8.GetBytes(GetKey());

    internal static string GetKey()
    {
        var key = TryGetKeyFromCLI();

        if (key.IsNot())
        {
            key = TryGetKeyFromAppSettings();
        }

        if (key.IsNot())
        {
            key = TryGetKeyFromEnvironmentVariable();
        }

        if (key.IsNot())
        {
            key = TryGetKeyFileName();
        }

        if (key.IsNot())
        {
            key = "ABCDEFGHIJKLMNOPQRST123456789011";

            if (CryptationKeyDirectory.Path.Is())
                Debug.Log("[Encryption] key is default 'ABC...' as 'Framework Enc Key File' is not found in FrameworkKeyDirectory");
            else
                Debug.Log("[Encryption] key is default 'ABC...' as no other framework enc key has been set");
        }

        KeyStart = key.MaxLength(3);

        return key.ToSha256Hash().MaxLength(47).Replace("-", "");
    }

    internal static string GetExceptionMessage(string cipherText)
    {
        return 
            $"Could not decrypt value starting with: {cipherText.MaxLength(4)}\n" +
            $"Tried decrypting with key: {KeyStart}...";
    }
}

internal static partial class CryptationKeyDirectory
{
    internal static string Path;
}