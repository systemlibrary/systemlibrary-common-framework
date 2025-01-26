using System.Text;

namespace SystemLibrary.Common.Framework;

internal static partial class CryptationKey
{
    internal static byte[] Instance;

    static object Lock = new object();

    internal static string FrameworkKeyDirectory;

    internal static string KeyStart;

    internal static byte[] Current
    {
        get
        {
            if (Instance == null)
            {
                lock (Lock)
                {
                    if (Instance != null) return Instance;

                    Instance = Encoding.UTF8.GetBytes(GetKey());
                }
            }
            return Instance;
        }
    }

    static string GetKey()
    {
        var key = TryGetKeyFileName();

        if(key.Is())
        {
            Debug.Log("Encryption key from key file: " + key.MaxLength(3) + "...");
        }
        else
        {
            key = AppInstance.AppName;

            if (key != "app")
            {
                Debug.Log("Encryption key is based on 'ApplicationName': " + key.MaxLength(3) + "...");
            }
            else
            {
                key = "ABCDEFGHIJKLMNOPQRST123456789011";

                if(FrameworkKeyDirectory.Is())
                    Debug.Log("Encryption key is default 'ABC...' as 'Framework Key File' is not found in FrameworkKeyDirectory, nor was 'ApplicationName' set in appSettings/AddDataProtection");
                else
                    Debug.Log("Encryption key is default 'ABC...' as 'FrameworkKeyDirectory' is not set in FrameworkBuilder.Create(), nor is 'ApplicationName' set in appSettings/AddDataProtection");
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
