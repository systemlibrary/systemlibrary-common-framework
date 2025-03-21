namespace SystemLibrary.Common.Framework;

partial class CryptationKey
{
    internal static string TryGetKeyFromEnvironmentVariable()
    {
        var key = Environment.GetEnvironmentVariable("frameworkEncKey") ??
             Environment.GetEnvironmentVariable("FrameworkEncKey") ??
             Environment.GetEnvironmentVariable("frameworkenckey") ??
             Environment.GetEnvironmentVariable("FRAMEWORKENCKEY");

        if (key.Is())
            Debug.Log("[Encryption] key is based on environment var: " + key.MaxLength(3) + "...");

        return key;
    }
}
