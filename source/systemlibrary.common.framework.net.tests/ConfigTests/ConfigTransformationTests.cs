using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.Tests;

namespace SystemLibrary.Common.Framework;

[TestClass]
public class ConfigTransformationTests : BaseTest
{
    [TestMethod]
    public void Transformation_Does_Not_Run_When_No_Transform_File_Exists_For_EnvName()
    {
        var config = Configs.EnvironmentConfig.Current;
        var environmentName = config.Name;

        if (environmentName == "PRODUCTION")
        {
            Assert.IsTrue(config.NewPropertyValue == "Hello world", "NewProperty has transformed to " + config.NewPropertyValue);
            Assert.IsTrue(config.NewString == "Production", "NewString has transformed to " + config.NewString);
            Assert.IsTrue(config.NewInt == 6, "Int has transformed to " + config.NewInt);
            Assert.IsTrue(config.NewFlag == false, "NewFlag has transformed to " + config.NewFlag);
            Assert.IsTrue(config.NewDateTime.ToString("yyyy-MM-dd HH:mm:ss") == "2024-12-24 13:44:15", "NewDateTime has transformed to " + config.NewDateTime);

            Assert.IsTrue(config.Child.Color == "orange");

            Assert.IsTrue(EnvironmentConfig.IsLocal == false);
            Assert.IsTrue(EnvironmentConfig.IsTest == false);
            Assert.IsTrue(EnvironmentConfig.IsProd == true);
        }
        else if (environmentName == "LOCAL")
        {
            Assert.IsTrue(config.NewPropertyValue == "Hello world", "NewProperty has transformed to " + config.NewPropertyValue);
            Assert.IsTrue(config.NewString == "Local", "NewString has transformed to " + config.NewString);
            Assert.IsTrue(config.NewInt == 5, "Int has transformed to " + config.NewInt);
            Assert.IsTrue(config.NewFlag == false, "NewFlag has transformed to " + config.NewFlag);
            Assert.IsTrue(config.NewDateTime.ToString("yyyy-MM-dd HH:mm:ss") == "2024-12-24 13:44:05", "NewDateTime has transformed to " + config.NewDateTime);

            Assert.IsTrue(config.Child.Color == "white");

            Assert.IsTrue(EnvironmentConfig.IsLocal == true);
            Assert.IsTrue(EnvironmentConfig.IsTest == false);
            Assert.IsTrue(EnvironmentConfig.IsProd == false);
        }
        else
        {
            if (environmentName.IsNot())
                Assert.IsTrue(false, "Please register a runtimesettings file in VS through Tes > 'Run Settings' with the environment name set through var ASPNETCORE_ENVIRONMENTNAME");
            else
                Assert.IsTrue(config.Name == "Untransformed", "Unknown configuration mode in env " + environmentName + ", should not transform any file - as no such transformation file exists, name: " + config.Name);
        }
    }
}
