using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.Net.Tests;

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

        Assert.IsTrue(conf.FirstName == "John Kusack");
        Assert.IsTrue(conf.lastname == "Doe");
        Assert.IsTrue(conf.FlagPascalCase);
        Assert.IsTrue(conf.FlagCamelCase);
        Assert.IsTrue(conf.Phone == 1234);
    }

    [TestMethod]
    public void Run_Multiple_Async_Calls_Success()
    {
        string Call(int i)
        {
            var key = i.ToString();
            var value = Configs.AppSettings.Current.Child.Color;
            return key + "=" + value;
        }

        var results = Async.Run(
            () => Call(1),
            () => Call(2),
            () => Call(3),
            () => Call(4),
            () => Call(5),
            () => Call(6),
            () => Call(7),
            () => Call(8));

        foreach (var result in results)
        {
            var parts = result.Split('=');
            var red = parts[1];

            Assert.IsTrue(red == "red", "Missing red in " + parts[0]);
        }
    }
}
