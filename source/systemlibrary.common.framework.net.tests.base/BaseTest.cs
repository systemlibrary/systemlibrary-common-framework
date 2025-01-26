using System.Reflection;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

using SystemLibrary.Common.Framework.App.Extensions;

namespace SystemLibrary.Common.Framework.Tests;

public abstract class BaseTest
{
    protected Assembly AssemblyPart;
    TestServer Server;

    HttpClient _Client;
    protected HttpClient Client
    {
        get
        {
            if (_Client == null)
            {
                var builder = new WebHostBuilder()
                   .ConfigureServices(services =>
                   {
                       ServiceOptions.ApplicationParts = [
                           AssemblyPart
                       ];

                       services = services.AddFrameworkServices<LogWriter>(ServiceOptions);
                   })
                   .Configure(app =>
                   {
                       app.UseFrameworkMiddlewares(null);
                   });

                Server = new TestServer(builder);

                _Client = Server.CreateClient();
            }
            return _Client;
        }
    }

    protected FrameworkServicesOptions ServiceOptions;

    [TestInitialize]
    public void Setup()
    {
        ServiceOptions = new FrameworkServicesOptions();
        ServiceOptions.UseHttpsRedirection = false;
    }

    public string GetResponse(string pathAndQuery)
    {
        var response = Client.GetAsync(pathAndQuery)
           .ConfigureAwait(false)
           .GetAwaiter()
           .GetResult();

        if(!response.IsSuccessStatusCode)
            Log.Dump("Not successful: " + pathAndQuery + " " +  response.StatusCode);

        return response.Content.ReadAsStringAsync()
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();
    }
}
