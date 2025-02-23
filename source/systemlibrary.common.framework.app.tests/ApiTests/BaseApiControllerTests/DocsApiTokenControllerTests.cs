using Microsoft.AspNetCore.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.App.Extensions;
using SystemLibrary.Common.Framework.Tests;

namespace SystemLibrary.Common.Framework.App;

[TestClass]
public class DocsApiTokenControllerTests : BaseTest
{
    public DocsApiTokenControllerTests()
    {
        WebHostBuilder = new WebHostBuilder()
            .ConfigureServices(services =>
            {
                var options = new FrameworkOptions();

                options.ApplicationParts = [
                    typeof(DocsApiTokenControllerTests).Assembly
                ];

                services = services.AddFrameworkServices(options);
            })
            .Configure(app =>
            {
                var options = new FrameworkOptions();

                options.UseHttpsRedirection = false;

                app.UseFrameworkMiddlewares(null, options);
            });
    }

    [TestMethod]
    public void Missing_Api_Token_Header_And_Value_Returns_NotOk()
    {
        var text = GetResponseText("/ApiTests/DocsApiToken/docs");

        IsNotOk(text);
    }

    [TestMethod]
    public void CaseInsensitive_Path_With_Api_Token_Header_And_Value_Returns_Ok()
    {
        var header = "api-token";

        var headerValue = "Docs";

        var text = GetResponseText("/ApiTests/DoCSApITokEn/dOCs", (header, headerValue));

        IsOk(text);
    }

    [TestMethod]
    public void Wrong_Header_Name_Returns_NotOk()
    {
        var header = "new-token";

        var headerValue = "Docs";

        var text = GetResponseText("/ApiTests/DoCSApITokEn/dOCs", (header, headerValue));

        IsNotOk(text);
    }
}
