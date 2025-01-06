using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace SystemLibrary.Common.Framework.Net.Tests;


public abstract class BaseTest
{
    WebHostBuilder WebHostBuilder;
    TestServer Server;

    HttpClient _Client;
    protected HttpClient Client
    {
        get
        {
            if (_Client == null)
            {
                Server = new TestServer(WebHostBuilder);
                _Client = Server.CreateClient();
            }

            return _Client;
        }
    }

    [TestInitialize]
    public void Setup()
    {
        var builder = new WebHostBuilder()
            .ConfigureServices(services =>
            {
                services.AddControllers();

                services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
                services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();
            })
            .Configure(app =>
            {
                app.UseRouting();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
            });
    }

    protected void True(bool flag, string message)
    {
        Assert.IsTrue(flag, message);
    }

    protected void False(bool flag, string message)
    {
        Assert.IsFalse(flag, message);
    }
}
