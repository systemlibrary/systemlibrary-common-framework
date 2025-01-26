using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.Tests;

namespace SystemLibrary.Common.Framework;

[TestClass]
public class EnvironmentConfigTests : BaseTest
{
    [TestMethod]
    public void EnvironmentConfig_Extending_Property_Has_Value()
    {
        var config = Configs.EnvironmentConfig.Current;

        Assert.IsTrue(config.NewPropertyValue == "Hello world", "Missing newPropertyValue");
    }

    [TestMethod]
    public void EnvironmentConfig_Name_Has_Correct_Value()
    {
        var config = Configs.EnvironmentConfig.Current;

        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "";

        Assert.IsTrue(config.Name.Is(), "EnvironmentName is not existing, mstest.runsettings has most likely not been added, select it in main menu: Test > Configure Run Settings > Select File ");
        Assert.IsTrue(config.Name == environment, "Environmnent name is wrong: " + config.Name + ", expected: " + environment);
    }

    [TestMethod]
    public void EnvironmentName_Is_Never_Null()
    {
        var config = EnvironmentConfig.Current;

        Assert.IsTrue(config.Name != null, "Environment.Name is null");
    }

    [TestMethod]
    public void Environment_Is_Test()
    {
        var config = EnvironmentConfig.Current;

        var env = config.Name.ToLower();

        var isTest = env == "qa" || env == "stage" || env == "test" || env == "at";

        if (isTest)
        {
            Assert.IsTrue(EnvironmentConfig.IsTest, "IsTest is false");
        }
        else
        {
            Assert.IsFalse(EnvironmentConfig.IsTest, "IsTest is true");
        }
    }

    [TestMethod]
    public void Environment_Is_Prod()
    {
        var config = EnvironmentConfig.Current;

        var env = config.Name.ToLower();

        var isProd = env == "prod" || env == "production";

        if (isProd)
        {
            Assert.IsTrue(EnvironmentConfig.IsProd, "IsProd is false");
        }
        else
        {
            Assert.IsFalse(EnvironmentConfig.IsProd, "IsProd is true");
        }
    }

    [TestMethod]
    public void Environment_Is_Local()
    {
        var config = EnvironmentConfig.Current;

        var env = config.Name.ToLower();

        var isProd = env == "prod" || env == "production";

        if (isProd)
        {
            Assert.IsTrue(EnvironmentConfig.IsProd, "IsProd is false");
        }
        else
        {
            Assert.IsFalse(EnvironmentConfig.IsProd, "IsProd is true");
        }
    }
}