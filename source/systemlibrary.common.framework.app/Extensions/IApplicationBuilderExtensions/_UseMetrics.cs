using Microsoft.AspNetCore.Builder;

using Prometheus;

using SystemLibrary.Common.Framework.Licensing;

namespace SystemLibrary.Common.Framework.App.Extensions;

partial class IApplicationBuilderExtensions
{
    static void UseMetrics(IApplicationBuilder app)
    {
        var enablePrometheusMetrics = AppSettings.Current.SystemLibraryCommonFramework.Metrics.Enable;
        if (enablePrometheusMetrics)
        {
            if (License.Gold())
            {
                app.UseEndpoints(endpoints =>
                {
                    Debug.Log("[MetricsMiddleware] Adding /metrics endpoint");

                    Metrics.SuppressDefaultMetrics();

                    endpoints.MapGet("/metrics", async context =>
                    {
                        if (!MetricsAuthorizationMiddleware.AuthorizeMetricsRequest(context)) return;

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
                });
            }
            else
            {
                Debug.Log("[MetricsMiddleware] Metrics enable is set to true, but license tier is not gold or above, not registering /metrics endpoint");
            }
        }
    }
}