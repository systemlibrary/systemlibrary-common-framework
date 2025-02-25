using System.Collections.Generic;
using System.Net.Http;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SystemLibrary.Common.Framework.App.Tests;

partial class ClientTests
{
    [TestMethod]
    public void Do_Throw_On_404()
    {
        var client = new Client(throwOnUnsuccessful: true);

        var url = "https://httpbin.org/status/404";

        var headers = GetHeaders();

        try
        {
            var data = client.Get<HttpResponseMessage>(url,
                ContentType.xwwwformUrlEncoded,
                headers: headers);

            Assert.IsTrue(false, "Should throw ex");
        }
        catch
        {
            Assert.IsTrue(true, "Threw exception");
        }
    }

    [TestMethod]
    public void Do_Throw_On_500()
    {
        var client = new Client(throwOnUnsuccessful: true);

        var url = "https://httpbin.org/status/500";

        var headers = GetHeaders();

        try
        {
            var data = client.Get<HttpResponseMessage>(url,
                ContentType.xwwwformUrlEncoded,
                headers: headers);

            Assert.IsTrue(false, "Should throw ex");
        }
        catch
        {
            Assert.IsTrue(true, "Threw exception");
        }
    }

    [TestMethod]
    public void Do_Not_Throw_On_404()
    {
        var client = new Client(throwOnUnsuccessful: false);

        var url = "https://httpbin.org/status/404";

        var headers = GetHeaders();

        var data = client.Get<HttpResponseMessage>(url,
            ContentType.xwwwformUrlEncoded,
            headers: headers);

        Assert.IsNotNull(data);
        Assert.IsTrue(data.StatusCode == data.Data.StatusCode);
    }

    [TestMethod]
    public void Do_Not_Throw_On_500()
    {
        var client = new Client(throwOnUnsuccessful: false);

        var url = "https://httpbin.org/status/500";

        var headers = GetHeaders();

        var data = client.Get<HttpResponseMessage>(url,
            ContentType.xwwwformUrlEncoded,
            headers: headers);

        Assert.IsNotNull(data);
        Assert.IsTrue(data.StatusCode == data.Data.StatusCode);
    }

    static IDictionary<string, string> GetHeaders()
    {
        return new Dictionary<string, string>()
        {
            { "accept", "*/*" },
            { "Connection", "Keep-Alive" }
        };
    }
}
