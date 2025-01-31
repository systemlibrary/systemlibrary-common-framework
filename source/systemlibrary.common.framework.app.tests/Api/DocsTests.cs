using System.Reflection;

using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace SystemLibrary.Common.Framework.App;

[TestClass]
public class DocsTests
{
    [TestMethod]
    public void Docs_Prints_TvContentApiControllers_Method_Names()
    {
        var controller = new TvContentApiController();

        var res = controller.Docs();

        var result = Log.Build(res).ToString();

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
    public void Execute_Api_Controllers_Docs_Success()
    {
        var controller = new UserApiController();

        var res = controller.Docs();
        var result = Log.Build(res).ToString();
    }
}
