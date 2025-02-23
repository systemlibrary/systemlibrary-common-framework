using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.App.Extensions;
using SystemLibrary.Common.Framework.Tests;

namespace SystemLibrary.Common.Framework.App;

[TestClass]
public class ServicesTests : BaseTest
{
    public ServicesTests()
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

                    var logwriter = Services.Get<ILogWriter>();

                    var httpContextAccessor = Services.Get<IHttpContextAccessor>();
                    var actionContextAccessor = Services.Get<IActionContextAccessor>();
                    
                    await context.Response.WriteAsync(logwriter.ToString() + actionContextAccessor.ToString() + httpContextAccessor.ToString());
                });
            });
    }

    [TestMethod]
    public void Services_Returns_Non_Null_Default_Services()
    {
        var text = GetResponseText("/");

        IsOk(text);

        IsOk(text.Contains("LogWriter"), "LogWriter missing");
        IsOk(text.Contains("HttpContextAccessor"), "HttpContextAccessor missing");
        IsOk(text.Contains("ActionContextAccessor"), "ActionContextAccessor missing");
    }
}