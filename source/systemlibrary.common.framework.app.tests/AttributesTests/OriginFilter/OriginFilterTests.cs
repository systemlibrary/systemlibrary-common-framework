using Microsoft.AspNetCore.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.App.Extensions;
using SystemLibrary.Common.Framework.Tests;

namespace SystemLibrary.Common.Framework.App;

[TestClass]
public class OriginFilterTests : BaseTest
{
    public OriginFilterTests()
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
    public void OriginFilter_Missing_Not_Match_Returns_Forbidden()
    {
        var path = "/api/originfilter?i=1";

        var response = GetResponseMessage(path);

        IsNotOk(response.StatusCode);
    }

    [TestMethod]
    public void OriginFilter_Blank_Not_Match_Returns_Forbidden()
    {
        var path = "/api/originfilter?i=1";

        var header = "oRiGin";

        var headerValue = "";

        var response = GetResponseMessage(path, (header, headerValue));

        IsNotOk(response.StatusCode);
    }

    [TestMethod]
    public void OriginFilter_Does_Not_Match_Returns_Forbidden()
    {
        var path = "/api/originfilter?i=1";

        var header = "oRiGin";

        var headerValue = "1234";
        
        var response = GetResponseMessage(path, (header, headerValue));

        IsNotOk(response.StatusCode);
    }

    [TestMethod]
    public void OriginFilter_Matches_Returns_OK()
    {
        var path = "/api/originfilter?i=1";

        var header = "oRiGin";

        var headerValue = "ABCdef123,.-_?$@#:;()<>!";

        var response = GetResponseMessage(path, (header, headerValue));

        IsOk(response.StatusCode);
    }

    [TestMethod]
    public void OriginFilterRegex_Matches_But_Too_Short_Returns_Forbidden()
    {
        var path = "/api/originfilterregex?i=1";

        var header = "oRiGin";

        var headerValue = "ab1";

        var response = GetResponseMessage(path, (header, headerValue));

        IsNotOk(response.StatusCode);
    }

    [TestMethod]
    public void OriginFilterRegex_Matches_Returns_OK()
    {
        var path = "/api/originfilterregex?i=1";

        var header = "oRiGin";

        var headerValue = "AB121212";

        var response = GetResponseMessage(path, (header, headerValue));

        IsOk(response.StatusCode);
    }

    [TestMethod]
    public void OriginFilterRegex_Does_Not_Match_Regex_Invalid_Chars_Returns_Forbidden()
    {
        var path = "/api/originfilterregex?i=1";

        var header = "oRiGin";

        var headerValue = "AB12125";

        var response = GetResponseMessage(path, (header, headerValue));

        IsNotOk(response.StatusCode);
    }
}
