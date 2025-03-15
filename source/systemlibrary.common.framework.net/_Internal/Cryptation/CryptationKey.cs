using System.Text;

namespace SystemLibrary.Common.Framework;

internal static partial class CryptationKey
{
    internal static readonly string _KeyFileFullName;

    internal static string KeyStart;

    internal static byte[] Current = Encoding.UTF8.GetBytes(GetKey());

    internal static string GetKey()
    {
        var key = TryGetFrameworkEncKey();

        if (key.IsNot())
        {
            key = TryGetKeyFileName();

            if (key.Is())
            {
                Debug.Log("[Encryption] key from key file: " + key.MaxLength(3) + "...");
            }
            else
            {
                key = "ABCDEFGHIJKLMNOPQRST123456789011";

                if (CryptationKeyDirectory.Path.Is())
                    Debug.Log("[Encryption] key is default 'ABC...' as 'Framework Enc Key File' is not found in FrameworkKeyDirectory");
                else
                    Debug.Log("[Encryption] key is default 'ABC...' as no other framework enc key has been set");
            }
        }

        KeyStart = key.MaxLength(3);

        return key.ToSha256Hash().MaxLength(47).Replace("-", "");
    }

    internal static string GetExceptionMessage(string cipherText)
    {
        var error = "Could not decrypt value starting with: " + cipherText.MaxLength(4);

        error += "\nTried decrypt with key starting with: " + KeyStart + "...";

        return error;
    }
}

internal static partial class CryptationKeyDirectory
{
    internal static string Path;
}