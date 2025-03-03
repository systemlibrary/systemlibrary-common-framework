﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.Tests;

namespace SystemLibrary.Common.Framework;

[TestClass]
public partial class StringExtensionsDPApiTests : BaseTest
{
    [TestMethod]
    public void Encrypt_And_Decrypt_Using_DataProtection_API_Success()
    {
        var data = "Hello world";

        var enc = data.EncryptUsingKeyRing();

        var enc2 = data.EncryptUsingKeyRing();

        var enc3 = data.EncryptUsingKeyRing();

        Assert.IsTrue(enc != data && enc.Length > data.Length + 10);

        var dec = enc.DecryptUsingKeyRing();

        Assert.IsTrue(dec == data, "Wrong decryption");
    }
}

