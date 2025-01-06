﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.Net.Tests;

namespace SystemLibrary.Common.Framework;

[TestClass]
public class FrameworkConfigTests : BaseTest
{
    [TestMethod]
    public void DumpConfig_Is_Set()
    {
        var config = FrameworkConfig.Current.Dump;

        Assert.IsTrue(config.FileName == "systemlibrary-common-framework-tests.log");
    }

    [TestMethod]
    public void JsonConfig_Is_Set()
    {
        var config = FrameworkConfig.Current.Json;

        Assert.IsTrue(config.MaxDepth == 99);
        Assert.IsTrue(!config.AllowTrailingCommas);
    }

    [TestMethod]
    public void CacheConfig_Is_Set()
    {
        var config = FrameworkConfig.Current.Cache;

        Assert.IsTrue(config != null);
    }

    [TestMethod]
    public void ClientConfig_Is_Set()
    {
        var config = FrameworkConfig.Current.Client;

        Assert.IsTrue(config != null);
    }
}