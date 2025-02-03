using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

using SystemLibrary.Common.Framework.App.Extensions;
using SystemLibrary.Common.Framework.Tests;

namespace SystemLibrary.Common.Framework.App;

public class AppBaseTest : BaseTest
{
    protected FrameworkServiceOptions FrameworkServicesOptions;
    protected FrameworkAppOptions FrameworkAppOptions;

    public AppBaseTest()
    {
        WebHostBuilder = new WebHostBuilder()
            .ConfigureServices(services =>
            {
                services = services.AddFrameworkServices<LogWriter>(FrameworkServicesOptions);
            })
            .Configure(app =>
            {
                if (FrameworkAppOptions == null)
                    FrameworkAppOptions = new FrameworkAppOptions();

                FrameworkAppOptions.UseHttpsRedirection = false;

                app.Use(async (context, next) =>
                {
                    try
                    {
                        var originalBodyStream = context.Response.Body;
                        using var responseBodyStream = new MemoryStream();
                        context.Response.Body = responseBodyStream;

                        await next();
                        if (context.Response.StatusCode >= 400)
                        {
                            responseBodyStream.Seek(0, SeekOrigin.Begin);
                            var responseText = await new StreamReader(responseBodyStream).ReadToEndAsync();
                            Log.Error(responseText);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);

                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("Internal Server Error");
                    }
                });

                app.UseFrameworkMiddlewares(null, FrameworkAppOptions);
            });
    }

   
}
