using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.Tests;

namespace SystemLibrary.Common.Framework;

[TestClass]
public class ConfigTests : BaseTest
{
    [TestMethod]
    public void NonExistingConfig_Is_Instanstiated()
    {
        var conf = NonExistingConfig.Current;

        Assert.IsTrue(conf != null);
    }

    [TestMethod]
    public void XmlConfig_Is_Read()
    {
        var conf = XmlConfig.Current;

        Assert.IsTrue(conf.FirstName == "John Kusack", "firstName missing " + conf.FirstName + ". Sure the XML is copied to output path on build, as the .exe is the starting point for the /Configs folders");
        Assert.IsTrue(conf.lastname == "Doe");
        Assert.IsTrue(conf.FlagPascalCase);
        Assert.IsTrue(conf.FlagCamelCase);
        Assert.IsTrue(conf.Phone == 1234);
    }
}
