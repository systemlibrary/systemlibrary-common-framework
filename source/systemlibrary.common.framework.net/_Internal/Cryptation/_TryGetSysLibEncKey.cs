namespace SystemLibrary.Common.Framework;

partial class CryptationKey
{
    internal static string TryGetFrameworkEncKey()
    {
        var key = TryGetEncKeyFromCommandLineArgs();

        if (key.Is())
        {
            Debug.Log("[Encryption] key is based on command line arg: " + key.MaxLength(3) + "...");
            return key;
        }

        key = TryGetEncKeyFromEnvironment();

        if (key.Is())
            Debug.Log("[Encryption] key is based on environment var: " + key.MaxLength(3) + "...");

        return key;
    }

    static string TryGetEncKeyFromEnvironment()
    {
        return Environment.GetEnvironmentVariable("frameworkEncKey") ??
             Environment.GetEnvironmentVariable("FrameworkEncKey") ??
             Environment.GetEnvironmentVariable("frameworkenckey") ??
             Environment.GetEnvironmentVariable("FRAMEWORKENCKEY");
    }

    static string TryGetEncKeyFromCommandLineArgs()
    {
        return Environment.GetCommandLineArgs()?.FirstOrDefault(a => a.StartsWith("frameworkEncKey=", StringComparison.OrdinalIgnoreCase))?.Split('=')[1];
    }
}
