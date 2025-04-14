using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

using Prometheus;

using SystemLibrary.Common.Framework.Licensing;

namespace SystemLibrary.Common.Framework.App.Extensions;

partial class IApplicationBuilderExtensions
{
    static DateTime? MetricLastReturned;
    static string MetricViewCached;

    static void UseMetrics(IApplicationBuilder app)
    {
        var isMetricsEnabled = AppSettings.Current.SystemLibraryCommonFramework.Metrics.Enable;
        if (isMetricsEnabled)
        {
            app.UseEndpoints(endpoints =>
            {
                Debug.Log("[Metrics] added /metrics and /metrics/ui");

                Metrics.SuppressDefaultMetrics();

                endpoints.MapGet("/metrics", async context =>
                {
                    if (!MetricsAuthorizationMiddleware.AuthorizeMetricsRequest(context)) return;

                    if (context.Request.Headers.ContainsKey("slcf-metrics-ui"))
                    {
                        if (MetricLastReturned > DateTime.Now.AddSeconds(-20))
                        {
                            context.Response.ContentType = "text/html";
                            context.Response.StatusCode = 200;

                            await context.Response.WriteAsync("");

                            return;
                        }
                        MetricLastReturned = DateTime.Now;
                    }

                    try
                    {
                        await Metrics.DefaultRegistry.CollectAndExportAsTextAsync(context.Response.Body);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);

                        throw;
                    }
                });

                endpoints.MapGet("/metrics/ui", async context =>
                {
                    if (!MetricsAuthorizationMiddleware.AuthorizeMetricsRequest(context)) return;

                    if (!License.Gold())
                    {
                        Debug.Log("[Metrics] enabled, but UI for metrics requires gold tier license or above");
                        await context.Response.WriteAsync("[Metrics] enabled, but UI for metrics requires gold tier license or above");
                        return;
                    }

                    if (!(MetricLastReturned > DateTime.Now.AddSeconds(-20)))
                    {
                        Debug.Log("[Metrics] recalculating");

                        var data = MetricsFetcher.Get(context);

                        MetricViewCached = MetricsUI.GetHtmlView(data);
                    }
                    else
                    {
                        Debug.Log("[Metrics] ui returned from 20s cache");
                    }

                    context.Response.ContentType = "text/html";

                    await context.Response.WriteAsync(MetricViewCached);
                });
            });
        }
    }
}