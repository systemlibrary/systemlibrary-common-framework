﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.Net.Tests;

namespace SystemLibrary.Common.Framework;

[TestClass]
public class AppSettingsTests : BaseTest
{
    [TestMethod]
    public void AppSettings_Deeply_Nested_Properties_Set_With_Case_Insensitive_Matching()
    {
        var config = Configs.AppSettings.Current;

        var child = config.Child;
        var grandChild = child.GrandChild;
        var greatGrandChild = grandChild.GreatGrandChild;
        var baby = greatGrandChild.Baby;

        Assert.IsTrue(child.Color == "red");
        Assert.IsTrue(grandChild.Color == "green");
        Assert.IsTrue(greatGrandChild.Color == "blue");
        Assert.IsTrue(baby.Color == "alpha");
        Assert.IsTrue(baby.number == 100);
    }

    [TestMethod]
    public void AppSettings_Sets_All_Matches_Case_Insensitive()
    {
        var config = Configs.AppSettings.Current;

        var username = config.username;
        var userName = config.userName;
        var USERNAME = config.USERNAME;
        var UserName = config.UserName;
        var Username = config.Username;

        Assert.IsTrue(username == "john.doe");
        Assert.IsTrue(userName == "john.doe");
        Assert.IsTrue(USERNAME == "john.doe");
        Assert.IsTrue(UserName == "john.doe");
        Assert.IsTrue(Username == "john.doe");
    }

    [TestMethod]
    public void AppSettings_All_Cases_Are_Set()
    {
        var config = Configs.AppSettings.Current;

        var PascalCase = config.PascalCase;
        var camelCase = config.camelCase;
        var UPPERCASE = config.UPPERCASE;
        var lowercase = config.lowercase;

        Assert.IsTrue(PascalCase == "PascalCase");
        Assert.IsTrue(camelCase == "camelCase");
        Assert.IsTrue(UPPERCASE == "UPPERCASE");
        Assert.IsTrue(lowercase == "lowercase");
    }
}
