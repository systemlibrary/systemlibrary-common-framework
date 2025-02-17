using System.Net;

using Microsoft.AspNetCore.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.App.Extensions;
using SystemLibrary.Common.Framework.Tests;

namespace SystemLibrary.Common.Framework.App;

[TestClass]
public class ApiTokenFilterTests : BaseTest
{
    public ApiTokenFilterTests()
    {
        WebHostBuilder = new WebHostBuilder()
            .ConfigureServices(services =>
            {
                services = services.AddFrameworkServices<LogWriter>();
            })
            .Configure(app =>
            {
                var options = new FrameworkAppOptions();

                options.UseHttpsRedirection = false;

                app.UseFrameworkMiddlewares(null, options);
            });
    }

    [TestMethod]
    public void ApiTokenFilter_Missing_Not_Match_Returns_Error()
    {
        var path = "/api/apitokenfilter?i=1";

        var response = GetResponseMessage(path);

        IsNotOk(response.StatusCode);
    }

    [TestMethod]
    public void ApiTokenFilter_Blank_Not_Match_Returns_Error()
    {
        var path = "/api/apitokenfilter?i=1";

        var header = "api-token";

        var headerValue = "";

        var response = GetResponseMessage(path, (header, headerValue));

        IsNotOk(response.StatusCode);
    }

    [TestMethod]
    public void ApiTokenFilter_Does_Not_Match_Returns_Error()
    {
        var path = "/api/apitokenfilter?i=1";

        var header = "api-token";

        var headerValue = "1234";

        var response = GetResponseMessage(path, (header, headerValue));

        IsNotOk(response.StatusCode);
    }

    [TestMethod]
    public void ApiTokenFilter_Matches_Returns_Success()
    {
        var path = "/api/apitokenfilter?i=1";

        var header = "api-token";

        var headerValue = "ABCdef123,.-_?$@#:;()<>!";

        var response = GetResponseMessage(path, (header, headerValue));

        IsOk(response.StatusCode);
    }
}
