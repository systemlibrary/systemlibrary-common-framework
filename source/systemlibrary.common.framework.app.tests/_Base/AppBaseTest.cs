using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

using SystemLibrary.Common.Framework.App.Extensions;
using SystemLibrary.Common.Framework.Tests;

namespace SystemLibrary.Common.Framework.App;

public class AppBaseTest : BaseTest
{
    protected FrameworkOptions FrameworkOptions;

    public AppBaseTest()
    {
        WebHostBuilder = new WebHostBuilder()
            .ConfigureServices(services =>
            {
                services = services.AddFrameworkServices<LogWriter>(FrameworkOptions);
            })
            .Configure(app =>
            {
                if (FrameworkOptions == null)
                    FrameworkOptions = new FrameworkOptions();

                FrameworkOptions.UseHttpsRedirection = false;

                app.Use(async (context, next) =>
                {
                    try
                    {
                        var originalBodyStream = context.Response.Body;
                        using var responseBodyStream = new MemoryStream();
                        context.Response.Body = responseBodyStream;

                        await next();

                        if (!context.Response.HasStarted)
                        {
                            context.Response.StatusCode = 200;
                            await context.Response.WriteAsync("Url: " + context.Request.Url());
                        }

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

                        await context.Response.WriteAsync("Internal Server Error: " + ex.Message);
                    }
                });

                app.UseFrameworkMiddlewares(null, FrameworkOptions);
            });
    }
}
