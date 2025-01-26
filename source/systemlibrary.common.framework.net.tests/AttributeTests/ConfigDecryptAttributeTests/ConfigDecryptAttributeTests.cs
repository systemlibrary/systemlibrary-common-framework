using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.Tests;

namespace SystemLibrary.Common.Framework;

[TestClass]
public class ConfigDecryptAttributeTests : BaseTest
{
    [TestMethod]
    public void Xml_Config_Decrypted()
    {
        var config = ConfigDecryptXmlConfig.Current;

        Assert.IsTrue(config.Password != "Hello world");
        Assert.IsTrue(config.Password2 != "Hello world");

        Assert.IsTrue(config.PasswordDecrypted == "Hello world");
        Assert.IsTrue(config.PasswordDecrypt == "Hello world");
        Assert.IsTrue(config.PasswordDecByAttrib == "Hello world");
        Assert.IsTrue(config.Password2DecByAttrib == "Hello world");
    }

    [TestMethod]
    public void Json_Config_Decrypted()
    {
        var config = ConfigDecryptJsonConfig.Current;

        Assert.IsTrue(config.Password != "Hello world");
        Assert.IsTrue(config.Password2 != "Hello world");

        Assert.IsTrue(config.PasswordDecrypted == "Hello world");
        Assert.IsTrue(config.PasswordDecrypt == "Hello world");
        Assert.IsTrue(config.PasswordDecByAttrib == "Hello world");
        Assert.IsTrue(config.Password2DecByAttrib == "Hello world");
    }
}
