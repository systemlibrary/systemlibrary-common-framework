﻿using Microsoft.AspNetCore.Hosting;
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
    public void GetPin()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/googleMaps/getPin");

        request.Headers.TryAddWithoutValidation("User-Agent", "He.l.lo-User-Agent;(SomeOS)");

        request.Headers.TryAddWithoutValidation("api-token", "helloworld");

        var response = Client.SendAsync(request)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

        Log.Dump(response.StatusCode);
        Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.OK);

        var text = response.Content.ReadAsStringAsync()
          .ConfigureAwait(false)
          .GetAwaiter()
          .GetResult();
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
