using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.App.Extensions;
using SystemLibrary.Common.Framework.Tests;

namespace SystemLibrary.Common.Framework.App;

[TestClass]
public class HttpRequestExtensionsTests : BaseTest
{
    public HttpRequestExtensionsTests()
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

                app.Run(async context =>
                {
                    var urlExtensionTest = context.Request.Url() + "\r\n" +
                                           context.Request.IsAjaxRequest() + "\r\n" +
                                           context.Request.Referer() + "\r\n" +
                                           context.Request.UserAgent() + "\r\n" +
                                           context.Request.ContentType() + "\r\n" +
                                           context.Request.Accept() + "\r\n";

                    await context.Response.WriteAsync(urlExtensionTest.ToString());
                });
            });
    }

    [TestMethod]
    public void Extensions_Does_Not_Throw_On_Null()
    {
        var referer = HttpContextInstance.Current.Request.Referer();

        IsOk(referer == null);
    }


    [TestMethod]
    public void Extension_Methods_Returns_Url_And_Empty_IsOk()
    {
        var text = GetResponseText("/hello/world/path?q=1");

        var expected = Assemblies.GetEmbeddedResource("Extension_Methods_Returns_Url_And_Empty_IsOk.txt");

        IsOk(text == expected, text + " VS " + expected);
    }

    [TestMethod]
    public void Extension_Methods_Returns_All_IsOk()
    {
        var text = GetResponseText("/hello/world/path?q=1",
            ("User-Agent", "agent1"),
            ("Content-Type", "app/content2"),
            ("Referer", "referer3"),
            ("Accept", "accept4"),
            ("X-Requested-With", "XMLHttpRequest"));

        var expected = Assemblies.GetEmbeddedResource("Extension_Methods_Returns_All_IsOk.txt");

        IsOk(text == expected, text + " VS " + expected);
    }
}