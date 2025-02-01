using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.App.Extensions;
using SystemLibrary.Common.Framework.Tests;

namespace SystemLibrary.Common.Framework.App;

[TestClass]
public class GoogleMapsTests : BaseTest
{
    public GoogleMapsTests()
    {
        WebHostBuilder = new WebHostBuilder()
            .ConfigureServices(services =>
            {
                var options = new FrameworkServicesOptions();

                services = services.AddFrameworkServices<LogWriter>(options);
            })
            .Configure(app =>
            {
                var options = new FrameworkAppOptions();

                options.UseHsts = false;
                options.UseHttpsRedirection = false;

                app.UseFrameworkMiddlewares(null, options);
            });
    }

    [TestMethod]
    public void GetPin_Fails_Invalid_User_Agent_403_Forbidden()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/googleMaps/GetPinOK/");

        request.Headers.TryAddWithoutValidation("User-Agent", "E2dg");

        request.Headers.TryAddWithoutValidation("api-token", "helloworld");

        var response = Client.SendAsync(request)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

        Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.Forbidden, "Not forbidden: " + response.StatusCode);
    }

    [TestMethod]
    public void GetPin_Fails_Invalid_Api_Token_403_Forbidden()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/googleMaps/GetPinOK/");

        request.Headers.TryAddWithoutValidation("User-Agent", "Edg");

        request.Headers.TryAddWithoutValidation("api-token", "hello2world");

        var response = Client.SendAsync(request)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

        Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.Forbidden, "Not forbidden: " + response.StatusCode);
    }


    [TestMethod]
    public void GetPin_Success_200_OK()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/googleMaps/GetPinOK/");

        request.Headers.TryAddWithoutValidation("User-Agent", "Edg");

        request.Headers.TryAddWithoutValidation("api-token", "helloworld");

        var response = Client.SendAsync(request)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

        Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.OK, "Not OK "  + response.StatusCode);

        var text = response.Content.ReadAsStringAsync()
          .ConfigureAwait(false)
          .GetAwaiter()
          .GetResult();

        Assert.IsTrue(text == "", "Text was something: " + text);
    }

    [TestMethod]
    public void GetPin_Returns_200_User_Agent_Is_Valid()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/userAgent/getPin/");

        request.Headers.TryAddWithoutValidation("User-Agent", "He.l.lo-User-Agent;(SomeOS)");

        var response = Client.SendAsync(request)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

        Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.OK);

        var text = response.Content.ReadAsStringAsync()
          .ConfigureAwait(false)
          .GetAwaiter()
          .GetResult();

        Assert.IsTrue(!text.Contains("403"), text);
    }
}
