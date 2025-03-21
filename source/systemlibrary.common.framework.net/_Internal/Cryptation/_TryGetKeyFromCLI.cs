namespace SystemLibrary.Common.Framework;

partial class CryptationKey
{
    internal static string TryGetKeyFromCLI()
    {
        var key = Environment.GetCommandLineArgs()?.FirstOrDefault(a => a.StartsWith("--frameworkEncKey=", StringComparison.OrdinalIgnoreCase))?.Split('=')[1];

        if (key.Is())
        {
            Debug.Log("[Encryption] key is based on command line arg: " + key.MaxLength(3) + "...");
            return key;
        }

        return null;
    }
}
