using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.App.Extensions;
using SystemLibrary.Common.Framework.Tests;

namespace SystemLibrary.Common.Framework.App;

[TestClass]
public class HtmlHelperFactoryTests : BaseTest
{
    public HtmlHelperFactoryTests()
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
                    var queryString = context.Request.QueryString.Value;

                    await context.Response.WriteAsync(queryString + "");
                });
            });
    }

    [TestMethod]
    public void Services_Returns_Non_Null_Default_Services()
    {
        var text = GetResponseText("/");
    }
}