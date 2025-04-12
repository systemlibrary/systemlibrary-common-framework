using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.App.Extensions;
using SystemLibrary.Common.Framework.Tests;

namespace SystemLibrary.Common.Framework.App;

[TestClass]
public class MetricTests : BaseTest
{
    public MetricTests()
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
    public void Run()
    {
        HostStart();


        Metric.Init(new MetricOption
        {
            DisplayLabel = "url",
            ShowAnimation = true,
            ShowLegend = true,
            ShowBorder = false,
            Slices = [
                new SliceOption
                {
                    Category = "api",
                    Status = "404",
                    Color = "orange",
                    Order = 1
                },

                new SliceOption
                {
                    Category = "api",
                    Status = "200",
                    Color = "green",
                    Order = 0
                },

                new SliceOption
                {
                    Category = "api",
                    Status = "500",
                    Color = "red",
                    Order = 2
                }
            ]
        });

        Metric.Inc("cache", "ssr", status: "hit");
        Metric.Inc("cache", "ssr", status: "hit");
        Metric.Inc("cache", "ssr", status: "hit");
        Metric.Inc("cache", "ssr", status: "hit");
        Metric.Inc("cache", "ssr", status: "miss");
        Metric.Inc("cache", "ssr", status: "fail");
        Metric.Inc("cache", "ssr", status: "fail");

        Metric.Inc("cache", "miss");
        Metric.Inc("cache", "hit");
        Metric.Inc("cache", "hit");
        Metric.Inc("cache", "hit");
        Metric.Inc("cache", "hit");
        Metric.Inc("cache", "hit");
        Metric.Inc("cache", "hit");
        Metric.Inc("cache", "miss");
        Metric.Inc("cache", "hit");
        Metric.Inc("url", "api", "500");
        Metric.Inc("url", "api", "404");
        Metric.Inc("url", "api", "404");
        Metric.Inc("url", "api", "200");
        Metric.Inc("url", "api", "200");
        Metric.Inc("url", "api", "200");
        Metric.Inc("url", "api", "301");
        Metric.Inc("url", "api", "200");
        Metric.Inc("cache", "hit");
        Metric.Inc("cache", "hit");
        Metric.Inc("cache", "hit");
        Metric.Inc("cache", "miss");
        Metric.Inc("loggedin");
        Metric.Inc("loggedin");
        Metric.Inc("loggedin");
        Metric.Inc("signedout");


        Thread.Sleep(60000);

        HostStop();
    }
}