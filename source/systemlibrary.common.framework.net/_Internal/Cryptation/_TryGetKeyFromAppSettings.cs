namespace SystemLibrary.Common.Framework;

partial class CryptationKey
{
    internal static string TryGetKeyFromAppSettings()
    {
        var key = FrameworkConfigInstance.Current.EncKey;

        if(key.Is())
            Debug.Log("{Encryption] key is read from appSettings " + key.MaxLength(3));

        return key;
    }
}
