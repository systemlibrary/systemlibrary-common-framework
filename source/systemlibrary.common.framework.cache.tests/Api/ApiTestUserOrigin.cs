using System.Net.Http;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SystemLibrary.Common.Framework.App.Tests;

[TestClass]
public class ApiTestUserOrigin
{
    TestServer _server;
    HttpClient _client;

    [TestInitialize]
    public void Setup()
    {
        var builder = new WebHostBuilder()
            .ConfigureServices(services =>
            {
                services.AddControllers(); // Registers controllers and filters
            })
            .Configure(app =>
            {
                app.UseRouting();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
            });

        _server = new TestServer(builder);

        _client = _server.CreateClient();
    }

    [TestMethod]
    public void Execute_Api_Returns_403_Not_Authorized()
    {
        var response = _client.GetAsync($"/userAgent/getPin/")
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();
        var txt = response.Content.ReadAsStringAsync()
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

        Assert.IsTrue(txt.Contains("403"), "No 403");

        var request2 = new HttpRequestMessage(HttpMethod.Get, "/userAgent/getPin/");
        request2.Headers.TryAddWithoutValidation("User-Agent", "He.l.lo-User-Agent;(SomeOS)");

        var response2 = _client.SendAsync(request2)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

        var txt2 = response2.Content.ReadAsStringAsync()
          .ConfigureAwait(false)
          .GetAwaiter()
          .GetResult();

        Assert.IsTrue(!txt2.Contains("403"), "Contains 403");
    }
}

[UserAgentFilter(match: "He.l.lo-User-Agent;(SomeOS)")]
public class UserAgentApiController : BaseApiController
{
    [HttpGet]
    [Route("/userAgent/getPin/")]
    public ActionResult GetPin() => Ok();
}
