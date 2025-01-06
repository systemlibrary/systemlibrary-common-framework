using System.Text;

namespace SystemLibrary.Common.Framework;

internal static partial class CryptationKey
{
    internal static byte[] _Key;

    static object Lock = new object();

    internal static string _KeyFileFullName;

    internal static string KeyStart;

    internal static byte[] Current
    {
        get
        {
            if (_Key == null)
            {
                lock (Lock)
                {
                    if (_Key != null) return _Key;

                    _Key = Encoding.UTF8.GetBytes(GetKey());
                }
            }
            return _Key;
        }
    }

    static string GetKey()
    {
        var key = TryGetKeyFromKeyFile();

        if(key.Is())
        {
            Debug.Log("Encryption Key from 'key file': " + key.MaxLength(3) + "...");
        }
        else
        {
            key = TryGetKeyFromAppName();

            if(key.Is())
            {
                Debug.Log("Encryption Key is based on 'app name': " + key.MaxLength(3) + "...");
            }
            else
            {
                key = "ABCDEFGHIJKLMNOPQRST123456789011";

                Debug.Log("Encryption Key is default 'ABC...' as 'Key File' is not found in any parent folder, nor is 'ApplicationName' set during AddDataProtection()");
            }
        }

        KeyStart = key.MaxLength(3);

        return key.ToSha256Hash().MaxLength(47).Replace("-", "");
    }

    internal static string GetExceptionMessage(string cipherText)
    {
        var error = "Could not decrypt value starting with: " + cipherText.MaxLength(4);

        error += "\nTried decrypt with key starting with: " + KeyStart.MaxLength(4) + "...";

        return error;
    }
}
