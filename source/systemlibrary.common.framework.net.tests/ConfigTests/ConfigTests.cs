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

    [TestMethod]
    public void Run_With_Timeout_Times_Out()
    {
        string Call(int i)
        {
            Thread.Sleep(333);
            var key = i.ToString();
            var value = Configs.AppSettings.Current.Child.Color;
            return key + "=" + value;
        }
        var results = Async.Run(100,
            () => Call(1),
            () => Call(2)
        );

        Assert.IsTrue(results.Count == 0, "Results returned items, it should return 0 as all times out");
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
