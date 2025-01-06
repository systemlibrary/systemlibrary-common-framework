namespace SystemLibrary.Common.Framework;

static internal class CryptationIV
{
    internal static byte[] Current
    {
        get
        {
            return Randomness.Bytes(16);
        }
    }
}
