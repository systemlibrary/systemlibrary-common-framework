using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.Tests;

namespace SystemLibrary.Common.Framework;

[TestClass]
public class FrameworkConfigTests : BaseTest
{
    [TestMethod]
    public void Is_Read_With_Case_Insensitive_Match()
    {
        var config = FrameworkConfigInstance.Current.Json;

        Assert.IsTrue(config.MaxDepth == 99, "Depth " + config.MaxDepth);
        Assert.IsTrue(!config.AllowTrailingCommas, "Commas " + config.AllowTrailingCommas);
     //   Assert.IsTrue(config.JsonCommentHandling == System.Text.Json.JsonCommentHandling.Skip);
        Assert.IsTrue(config.IncludeFields);
        Assert.IsTrue(config.JsonIgnoreCondition == System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault);
    }

    [TestMethod]
    public void CacheConfig_Is_Set()
    {
        var config = FrameworkConfigInstance.Current.Cache;

        Assert.IsTrue(config != null);
    }

    [TestMethod]
    public void ClientConfig_Is_Set()
    {
        var config = FrameworkConfigInstance.Current.Client;

        Assert.IsTrue(config != null);
    }
}
