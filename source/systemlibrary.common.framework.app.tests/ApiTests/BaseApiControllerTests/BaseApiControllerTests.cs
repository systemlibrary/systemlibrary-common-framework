using Microsoft.AspNetCore.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.App.Extensions;
using SystemLibrary.Common.Framework.Tests;

namespace SystemLibrary.Common.Framework.App;

[TestClass]
public class BaseApiControllerTests : BaseTest
{
    public BaseApiControllerTests()
    {
        WebHostBuilder = new WebHostBuilder()
            .ConfigureServices(services =>
            {
                services = services.AddFrameworkServices();
            })
            .Configure(app =>
            {
                var options = new FrameworkAppOptions();

                options.UseHttpsRedirection = false;

                app.UseFrameworkMiddlewares(null, options);
            });
    }

    [TestMethod]
    public void Default_Api_Routing_For_BaseApiControllers_Without_Api_Token_Returns_NotOk()
    {
        
        var text = GetResponseText("/ApiTests/DocsApiToken/docs");

        IsNotOk(text);
    }

    [TestMethod]
    public void Last_Namespace_Segment_Routing_Case_Insensitive_For_BaseApiControllers_Returns_Ok()
    {
        var header = "api-token";

        var headerValue = "Docs";

        var text = GetResponseText("/ApiTests/DoCSApITokEn/dOCs", (header, headerValue));

        IsOk(text);
    }

    [TestMethod]
    public void DocsController_Returns_Formatted_Docs()
    {
        var text = GetResponseText("/docs/docs");

        var expectedLines = Assemblies.GetEmbeddedResource("DocsFormat.json").Split(Environment.NewLine);

        foreach(var line in expectedLines)
        {
            IsOk(text.Contains(line), "Line missing or invalid: " + line);
        }
        IsOk(text);
    }
}
