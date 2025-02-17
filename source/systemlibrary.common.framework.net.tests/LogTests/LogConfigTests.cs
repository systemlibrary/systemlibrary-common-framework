using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.Tests;

namespace SystemLibrary.Common.Framework;

[TestClass]
public class LogConfigTests : BaseTest
{
    const string DumpFullPath = @"%HomeDrive%\Logs\systemlibrary-common-framework-tests.log";

    [TestMethod]
    public void LogConfig_Read_Properly_With_Default_Values_Overwritten()
    {
        var config = FrameworkConfigInstance.Current.Log;

        Assert.IsTrue(config.FullFilePath.Contains(DumpFullPath), config.FullFilePath);

        Assert.IsTrue(config.Level == Log.LogLevel.Information, "Wrong level " + config.Level);

        Assert.IsTrue(config.Level == Log.LogLevel.Information, "Wrong level " + config.Level);

        Assert.IsTrue(config.Format == Log.LogFormat.Json, "Wrong format " + config.Format);

        Assert.IsTrue(config.AddUrl, "Wrong url " + config.AddUrl);
        Assert.IsTrue(!config.AddHttpMethod, "Wrong addHttpMethod " + config.AddHttpMethod);
        Assert.IsTrue(config.AddAuthenticatedState, "Wrong AddAuthenticatedState " + config.AddAuthenticatedState);
    }
}
