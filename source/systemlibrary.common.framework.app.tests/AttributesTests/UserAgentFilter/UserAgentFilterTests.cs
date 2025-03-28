using Microsoft.AspNetCore.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.App.Extensions;
using SystemLibrary.Common.Framework.Tests;

namespace SystemLibrary.Common.Framework.App;

[TestClass]
public class UserAgentFilterTests : BaseTest
{
    [TestMethod]
    public void AConfigTests()
    {
        var a  = AConfig.Current;

        Assert.IsTrue(a != null, "A is null");

        Assert.IsTrue(a.ApiUrl == "www", "Api Url is " + a.ApiUrl);
    }

    public UserAgentFilterTests()
    {
        WebHostBuilder = new WebHostBuilder()
            .ConfigureServices(services =>
            {
                services = services.AddFrameworkServices<LogWriter>();
            })
            .Configure(app =>
            {
                var options = new FrameworkOptions();

                options.UseHttpsRedirection = false;

                app.UseFrameworkMiddlewares(null, options);
            });
    }

    [TestMethod]
    public void Missing_UserAgent_Returns_Forbidden()
    {
        var path = "/api/UserAgentFilter?i=1";

        var response = GetResponseMessage(path);

        IsNotOk(response.StatusCode);
    }

    [TestMethod]
    public void Blank_UserAgent_Returns_Forbidden()
    {
        var path = "/api/UserAgentFilter?i=1";

        var header = "USER-agEnt";

        var headerValue = "";

        var response = GetResponseMessage(path, (header, headerValue));

        IsNotOk(response.StatusCode);
    }

    [TestMethod]
    public void Invalid_Casing_UserAgent_Returns_Forbidden()
    {
        var path = "/api/userAgentFilter?i=1";

        var header = "USER-agEnt";

        var headerValue = "abcDEF123,.-_?$@#:;()<>!";

        var response = GetResponseMessage(path, (header, headerValue));

        IsNotOk(response.StatusCode);
    }

    [TestMethod]
    public void Valid_UserAgent_Returns_Forbidden()
    {
        var path = "/api/userAgentFilter?i=1";

        var header = "USER-agEnt";

        var headerValue = "ABCdef123,.-_?$@#:;()<>!";

        var response = GetResponseMessage(path, (header, headerValue));

        IsOk(response.StatusCode);
    }

    [TestMethod]
    public void Invalid_UserAgent_Returns_Forbidden()
    {
        var path = "/api/userAgentFilter?i=1";

        var header = "USER-agEnt";

        var headerValue = "ABCdef123,.-_?$@#:;()"; // missing 3 chars

        var response = GetResponseMessage(path, (header, headerValue));

        IsNotOk(response.StatusCode);
    }
}
