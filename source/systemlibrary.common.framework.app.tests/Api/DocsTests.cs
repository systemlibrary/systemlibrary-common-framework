﻿using System.Reflection;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SystemLibrary.Common.Framework.App;

[TestClass]
public class DocsTests
{
    [TestMethod]
    public void Docs_Prints_TvContentApiController_Methods()
    {
        var controller = new TvContentApiController();

        var res = controller.Docs();

        var result = Log.Build(res).ToString();

        Log.Dump(result);
        var methods = typeof(TvContentApiController).GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

        var apiPathPrefix = typeof(TvContentApiController).Name.Replace("Controller", "/");

        foreach (var method in methods)
        {
            if (method.Name == "Docs") continue;

            var hasParams = method.GetParameters();

            var testName = apiPathPrefix + method.Name;

            if (hasParams?.Length > 0)
            {
                testName += "?";
            }

            Assert.IsTrue(result.Contains(testName, StringComparison.OrdinalIgnoreCase), method.Name + " missing");
        }
    }

    [TestMethod]
    public void Docs_Prints_UserApiController_Methods()
    {
        var controller = new UserApiController();

        var res = controller.Docs();

        var result = Log.Build(res).ToString();

        var methods = typeof(UserApiController).GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

        var apiPathPrefix = typeof(UserApiController).Name.Replace("Controller", "/");

        foreach (var method in methods)
        {
            if (method.Name == "Docs") continue;

            var hasParams = method.GetParameters();

            var testName = apiPathPrefix + method.Name;

            if (hasParams?.Length > 0)
            {
                testName += "?";
            }

            Assert.IsTrue(result.Contains(testName, StringComparison.OrdinalIgnoreCase), method.Name + " missing");
        }
    }
}
