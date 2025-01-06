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
        var key = TryGetKeyFromDataRingKeyFile();

        if (key.IsNot())
        {
            key = TryGetKeyFromAppNameOrAsmName();

            if (key != null)
                Debug.Log("Encryption Key is based on 'app/asm name': " + key.MaxLength(5));
        }
        else
        {
            Debug.Log("Encryption Key from 'key ring file': " + key.MaxLength(4));
        }
        
        if (key.IsNot())
        {
            key = "ABCDEFGHIJKLMNOPQRST123456789011";

            Debug.Log("Encryption Key is default (ABC...) as 'AddDataProtection' service is not registered and no key ring file found in any parent folder of Content Root");

            KeyStart = key.MaxLength(maxLength: 4);
        }
        else
        {
            KeyStart = key.MaxLength(maxLength: 2);
        }
        return key.ToSha256Hash().MaxLength(47).Replace("-", "");
    }

    internal static string GetExceptionMessage(string cipherText)
    {
        var error = "Could not decrypt value starting with: " + cipherText.MaxLength(4);

        error += "\nTried decrypt with key starting with: " + KeyStart.MaxLength(4) + "...";

        return error;
    }
}
