using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.Net.Tests;

namespace SystemLibrary.Common.Framework;

[TestClass]
public partial class StringExtensionsDPApiTests : BaseTest
{
    [TestInitialize]
    public void TestInitialize()
    {
        CryptationKey._Key = null;
    }

    [TestMethod]
    public void Encrypt_And_Decrypt_Using_DataProtection_API_Success()
    {
        var data = "Hello world";

        var enc = data.EncryptUsingKeyRing();

        var enc2 = data.EncryptUsingKeyRing();

        var enc3 = data.EncryptUsingKeyRing();

        Assert.IsTrue(enc != data && enc.Length > data.Length + 10);

        Dump.Write(enc3);

        var dec = enc.DecryptUsingKeyRing();

        Assert.IsTrue(dec == data, "Wrong decryption");
    }
}

