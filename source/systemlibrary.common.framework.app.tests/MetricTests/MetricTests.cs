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

        MetricCharts.Add(new MetricChartOption
        {
            MetricLabel = "www.vg.no:success",
            ShowAnimation = true,
            ShowLegend = true,
            ShowBorder = false,
            TextColor = "#f1f1f1",
            Slices = [
                new SliceOption
                {
                    Segment = "200",
                    Color = "orange",
                    Order = 155
                },
                new SliceOption
                {
                    Segment = "404",
                    Color = "red",
                    Order = 4
                },
                new SliceOption
                {
                    Segment = "301",
                    Color = "black",
                    Order = 1
                }
            ]
        });

        Metric.Inc("www.vg.no:circuit_broken", "0");
        Metric.Inc("www.vg.no:success", "200");
        Metric.Inc("www.vg.no:success", "200");
        Metric.Inc("www.vg.no:success", "301");
        Metric.Inc("www.vg.no:success", "404");
        Metric.Inc("www.vg.no:retry_success", "200");
        Metric.Inc("www.vg.no:retry_success", "200");
        Metric.Inc("www.vg.no:retry_success", "200");
        Metric.Inc("www.vg.no:failed", "404");
        Metric.Inc("www.vg.no:failed", "500");
        Metric.Inc("www.vg.no:failed", "500");
        Metric.Inc("www.vg.no:failed", "500");
        Metric.Inc("www.vg.no:failed", "500");

        Metric.Inc("www.vg.no:badgateway");
        Metric.Inc("www.vg.no:badgateway");

        Metric.Inc("cache:ssr", "hit");
        Metric.Inc("cache:ssr", "hit");
        Metric.Inc("cache:ssr", "hit");
        Metric.Inc("cache:ssr", "hit");
        Metric.Inc("cache:ssr", "miss");
        Metric.Inc("cache:ssr", "fail");
        Metric.Inc("cache:ssr", "fail");
        Metric.Inc("cache:fallback", "fail");
        Metric.Inc("cache:fallback", "hit");

        Metric.Inc("cache:hit");
        Metric.Inc("cache:miss");


        Metric.Inc("www.vg.no:circuit_broken", "0");
        Metric.Inc("www.vg.no:success", "200");
        Metric.Inc("www.vg.no:success", "200");
        Metric.Inc("www.vg.no:success", "301");
        Metric.Inc("www.vg.no:success", "404");
        Metric.Inc("www.vg.no:retry_success", "200");
        Metric.Inc("www.vg.no:retry_success", "200");
        Metric.Inc("www.vg.no:retry_success", "200");
        Metric.Inc("www.vg.no:failed", "404");
        Metric.Inc("www.vg.no:failed", "500");
        Metric.Inc("www.vg.no:failed", "500");
        Metric.Inc("www.vg.no:failed", "500");
        Metric.Inc("www.vg.no:failed", "500");

        Metric.Inc("www.vg.no:badgateway");
        Metric.Inc("www.vg.no:badgateway");

        Metric.Inc("cache:ssr", "hit");
        Metric.Inc("cache:ssr", "hit");
        Metric.Inc("cache:ssr", "hit");
        Metric.Inc("cache:ssr", "hit");
        Metric.Inc("cache:ssr", "miss");
        Metric.Inc("cache:ssr", "fail");
        Metric.Inc("cache:ssr", "fail");
        Metric.Inc("cache:fallback", "fail");
        Metric.Inc("cache:fallback", "hit");

        Metric.Inc("cache:hit");
        Metric.Inc("cache:miss");

        Metric.Inc("www.vg.no:circuit_broken", "0");
        Metric.Inc("www.vg.no:success", "200");
        Metric.Inc("www.vg.no:success", "200");
        Metric.Inc("www.vg.no:success", "301");
        Metric.Inc("www.vg.no:success", "404");
        Metric.Inc("www.vg.no:retry_success", "200");
        Metric.Inc("www.vg.no:retry_success", "200");
        Metric.Inc("www.vg.no:retry_success", "200");
        Metric.Inc("www.vg.no:failed", "404");
        Metric.Inc("www.vg.no:failed", "500");
        Metric.Inc("www.vg.no:failed", "500");
        Metric.Inc("www.vg.no:failed", "500");
        Metric.Inc("www.vg.no:failed", "500");

        Metric.Inc("www.vg.no:badgateway");
        Metric.Inc("www.vg.no:badgateway");

        Metric.Inc("cache:ssr", "hit");
        Metric.Inc("cache:ssr", "hit");
        Metric.Inc("cache:ssr", "hit");
        Metric.Inc("cache:ssr", "hit");
        Metric.Inc("cache:ssr", "miss");
        Metric.Inc("cache:ssr", "fail");
        Metric.Inc("cache:ssr", "fail");
        Metric.Inc("cache:fallback", "fail");
        Metric.Inc("cache:fallback", "hit");

        Metric.Inc("cache:hit");
        Metric.Inc("cache:miss");

        Metric.Inc("www.vg.no:circuit_broken", "0");
        Metric.Inc("www.vg.no:success", "200");
        Metric.Inc("www.vg.no:success", "200");
        Metric.Inc("www.vg.no:success", "301");
        Metric.Inc("www.vg.no:success", "404");
        Metric.Inc("www.vg.no:retry_success", "200");
        Metric.Inc("www.vg.no:retry_success", "200");
        Metric.Inc("www.vg.no:retry_success", "200");
        Metric.Inc("www.vg.no:failed", "404");
        Metric.Inc("www.vg.no:failed", "500");
        Metric.Inc("www.vg.no:failed", "500");
        Metric.Inc("www.vg.no:failed", "500");
        Metric.Inc("www.vg.no:failed", "500");

        Metric.Inc("www.vg.no:badgateway");
        Metric.Inc("www.vg.no:badgateway");

        Metric.Inc("cache:ssr", "hit");
        Metric.Inc("cache:ssr", "hit");
        Metric.Inc("cache:ssr", "hit");
        Metric.Inc("cache:ssr", "hit");
        Metric.Inc("cache:ssr", "miss");
        Metric.Inc("cache:ssr", "fail");
        Metric.Inc("cache:ssr", "fail");
        Metric.Inc("cache:fallback", "fail");
        Metric.Inc("cache:fallback", "hit");

        Metric.Inc("cache:hit");
        Metric.Inc("cache:miss");

        Metric.Inc("www.vg.no:circuit_broken", "0");
        Metric.Inc("www.vg.no:success", "200");
        Metric.Inc("www.vg.no:success", "200");
        Metric.Inc("www.vg.no:success", "301");
        Metric.Inc("www.vg.no:success", "404");
        Metric.Inc("www.vg.no:retry_success", "200");
        Metric.Inc("www.vg.no:retry_success", "200");
        Metric.Inc("www.vg.no:retry_success", "200");
        Metric.Inc("www.vg.no:failed", "404");
        Metric.Inc("www.vg.no:failed", "500");
        Metric.Inc("www.vg.no:failed", "500");
        Metric.Inc("www.vg.no:failed", "500");
        Metric.Inc("www.vg.no:failed", "500");

        Metric.Inc("www.vg.no:badgateway");
        Metric.Inc("www.vg.no:badgateway");

        Metric.Inc("cache:ssr", "hit");
        Metric.Inc("cache:ssr", "hit");
        Metric.Inc("cache:ssr", "hit");
        Metric.Inc("cache:ssr", "hit");
        Metric.Inc("cache:ssr", "miss");
        Metric.Inc("cache:ssr", "fail");
        Metric.Inc("cache:ssr", "fail");
        Metric.Inc("cache:fallback", "fail");
        Metric.Inc("cache:fallback", "hit");

        Metric.Inc("cache:hit");
        Metric.Inc("cache:miss");

        Metric.Inc("www.vg.no:circuit_broken", "0");
        Metric.Inc("www.vg.no:success", "200");
        Metric.Inc("www.vg.no:success", "200");
        Metric.Inc("www.vg.no:success", "301");
        Metric.Inc("www.vg.no:success", "404");
        Metric.Inc("www.vg.no:retry_success", "200");
        Metric.Inc("www.vg.no:retry_success", "200");
        Metric.Inc("www.vg.no:retry_success", "200");
        Metric.Inc("www.vg.no:failed", "404");
        Metric.Inc("www.vg.no:failed", "500");
        Metric.Inc("www.vg.no:failed", "500");
        Metric.Inc("www.vg.no:failed", "500");
        Metric.Inc("www.vg.no:failed", "500");

        Metric.Inc("www.vg.no:badgateway");
        Metric.Inc("www.vg.no:badgateway");

        Metric.Inc("cache:ssr", "hit");
        Metric.Inc("cache:ssr", "hit");
        Metric.Inc("cache:ssr", "hit");
        Metric.Inc("cache:ssr", "hit");
        Metric.Inc("cache:ssr", "miss");
        Metric.Inc("cache:ssr", "fail");
        Metric.Inc("cache:ssr", "fail");
        Metric.Inc("cache:fallback", "fail");
        Metric.Inc("cache:fallback", "hit");

        Metric.Inc("cache:hit");
        Metric.Inc("cache:miss");

        Metric.Inc("www.vg.no:circuit_broken", "0");
        Metric.Inc("www.vg.no:success", "200");
        Metric.Inc("www.vg.no:success", "200");
        Metric.Inc("www.vg.no:success", "301");
        Metric.Inc("www.vg.no:success", "404");
        Metric.Inc("www.vg.no:retry_success", "200");
        Metric.Inc("www.vg.no:retry_success", "200");
        Metric.Inc("www.vg.no:retry_success", "200");
        Metric.Inc("www.vg.no:failed", "404");
        Metric.Inc("www.vg.no:failed", "500");
        Metric.Inc("www.vg.no:failed", "500");
        Metric.Inc("www.vg.no:failed", "500");
        Metric.Inc("www.vg.no:failed", "500");

        Metric.Inc("www.vg.no:badgateway");
        Metric.Inc("www.vg.no:badgateway");

        Metric.Inc("cache:ssr", "hit");
        Metric.Inc("cache:ssr", "hit");
        Metric.Inc("cache:ssr", "hit");
        Metric.Inc("cache:ssr", "hit");
        Metric.Inc("cache:ssr", "miss");
        Metric.Inc("cache:ssr", "fail");
        Metric.Inc("cache:ssr", "fail");
        Metric.Inc("cache:fallback", "fail");
        Metric.Inc("cache:fallback", "hit");

        Metric.Inc("cache:hit");
        Metric.Inc("cache:miss");

        Metric.Inc("www.vg.no:circuit_broken", "0");
        Metric.Inc("www.vg.no:success", "200");
        Metric.Inc("www.vg.no:success", "200");
        Metric.Inc("www.vg.no:success", "301");
        Metric.Inc("www.vg.no:success", "404");
        Metric.Inc("www.vg.no:retry_success", "200");
        Metric.Inc("www.vg.no:retry_success", "200");
        Metric.Inc("www.vg.no:retry_success", "200");
        Metric.Inc("www.vg.no:failed", "404");
        Metric.Inc("www.vg.no:failed", "500");
        Metric.Inc("www.vg.no:failed", "500");
        Metric.Inc("www.vg.no:failed", "500");
        Metric.Inc("www.vg.no:failed", "500");

        Metric.Inc("www.vg.no:badgateway");
        Metric.Inc("www.vg.no:badgateway");

        Metric.Inc("cache:ssr", "hit");
        Metric.Inc("cache:ssr", "hit");
        Metric.Inc("cache:ssr", "hit");
        Metric.Inc("cache:ssr", "hit");
        Metric.Inc("cache:ssr", "miss");
        Metric.Inc("cache:ssr", "fail");
        Metric.Inc("cache:ssr", "fail");
        Metric.Inc("cache:fallback", "fail");
        Metric.Inc("cache:fallback", "hit");

        Metric.Inc("cache:hit");
        Metric.Inc("cache:miss");

        Metric.Inc("www.vg.no:circuit_broken", "0");
        Metric.Inc("www.vg.no:success", "200");
        Metric.Inc("www.vg.no:success", "200");
        Metric.Inc("www.vg.no:success", "301");
        Metric.Inc("www.vg.no:success", "404");
        Metric.Inc("www.vg.no:retry_success", "200");
        Metric.Inc("www.vg.no:retry_success", "200");
        Metric.Inc("www.vg.no:retry_success", "200");
        Metric.Inc("www.vg.no:failed", "404");
        Metric.Inc("www.vg.no:failed", "500");
        Metric.Inc("www.vg.no:failed", "500");
        Metric.Inc("www.vg.no:failed", "500");
        Metric.Inc("www.vg.no:failed", "500");

        Metric.Inc("www.vg.no:badgateway");
        Metric.Inc("www.vg.no:badgateway");

        Metric.Inc("cache:ssr", "hit");
        Metric.Inc("cache:ssr", "hit");
        Metric.Inc("cache:ssr", "hit");
        Metric.Inc("cache:ssr", "hit");
        Metric.Inc("cache:ssr", "miss");
        Metric.Inc("cache:ssr", "fail");
        Metric.Inc("cache:ssr", "fail");
        Metric.Inc("cache:fallback", "fail");
        Metric.Inc("cache:fallback", "hit");

        Metric.Inc("cache:hit");
        Metric.Inc("cache:miss");

        Thread.Sleep(25000);

        HostStop();
    }
}