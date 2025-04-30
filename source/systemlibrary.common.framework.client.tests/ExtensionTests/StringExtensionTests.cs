using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.App.Extensions;
using SystemLibrary.Common.Framework.Tests;

namespace SystemLibrary.Common.Framework.App.Tests;

[TestClass]
public partial class StringExtensionTests : BaseTest
{
    [TestMethod]
    public void Get_Success()
    {
        // NOTE: This takes approx 360ms
        var response = "https://www.google.com".Get<string>().Data;
        
        IsOk(response);

        // NOTE: This takes approx 300ms, am I wasting roughly 60ms in generating cache and various handlers? It cant be?
        //var handler = new SocketsHttpHandler
        //{
        //    PooledConnectionLifetime = TimeSpan.FromMinutes(10),
        //    PooledConnectionIdleTimeout = TimeSpan.FromMinutes(5),
        //    EnableMultipleHttp2Connections = true,
        //    UseProxy = false,
        //};

        //var client = new HttpClient(handler)
        //{
        //    Timeout = TimeSpan.FromSeconds(5)
        //};

        //var request = new HttpRequestMessage(HttpMethod.Get, "https://www.google.com")
        //{
        //    Version = HttpVersion.Version20,
        //    VersionPolicy = HttpVersionPolicy.RequestVersionOrHigher
        //};

        //var response = client.SendAsync(request).ConfigureAwait(false).GetAwaiter().GetResult();
        //var content = response.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult();

        //IsOk(content);
    }

    [TestMethod]
    public void Post_Success()
    {
        var response = "https://httpbin.org/post".Post<string>("hello world");

        IsOk(response.StatusCode);
        IsOk(response.Data);
    }

    [TestMethod]
    public void Put_Success()
    {
        var response = "https://httpbin.org/put".Put<string>("hello world");

        IsOk(response.StatusCode);
        IsOk(response.Data);
    }

    [TestMethod]
    public void Delete_Success()
    {
        var response = "https://httpbin.org/delete".Delete<string>("hello world");

        IsOk(response.StatusCode);
        IsOk(response.Data);
    }
}