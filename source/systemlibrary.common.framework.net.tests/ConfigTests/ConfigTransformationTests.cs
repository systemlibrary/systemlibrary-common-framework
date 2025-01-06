using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.Net.Tests;

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
        }
        else if (environmentName == "LOCAL")
        {
            Assert.IsTrue(config.NewPropertyValue == "Hello world", "NewProperty has transformed to " + config.NewPropertyValue);
            Assert.IsTrue(config.NewString == "Local", "NewString has transformed to " + config.NewString);
            Assert.IsTrue(config.NewInt == 5, "Int has transformed to " + config.NewInt);
            Assert.IsTrue(config.NewFlag == false, "NewFlag has transformed to " + config.NewFlag);
            Assert.IsTrue(config.NewDateTime.ToString("yyyy-MM-dd HH:mm:ss") == "2024-12-24 13:44:05", "NewDateTime has transformed to " + config.NewDateTime);

            Assert.IsTrue(config.Child.Color == "white");
        }
        else
        {
            Assert.IsTrue(config.Name == "Untransformed", "Unknown configuration mode, should not transform any file - as no such transformation file exists, name: " + config.Name);
        }
    }
}
