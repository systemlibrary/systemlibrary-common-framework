using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SystemLibrary.Common.Framework.App.Tests;

partial class ClientTests
{
    [TestMethod]
    public void Post_Plain_Text_Success()
    {
        var bin = new HttpBin();

        var response = bin.Post("hello world", MediaType.plain);

        Assert.IsTrue(response.Data.Contains("hello world"));
    }

    [TestMethod]
    public void Post_Url_Encoded_Success()
    {
        var bin = new HttpBin();

        var response = bin.PostUrlEncoded("hello=world&hello2=world2");

        var form = response.Data.JsonPartial<Form>();

        Assert.IsTrue(response.Data.Contains("urlencoded"));
        Assert.IsTrue(form != null, "!form posted");
        Assert.IsTrue(form.hello.Is());
    }

    [TestMethod]
    public void Post_Json_Success()
    {
        var bin = new HttpBin();

        var response = bin.Post("{ hello:\"world\" }", MediaType.json);

        Assert.IsTrue(response.Data.Contains("world"));
    }

    [TestMethod]
    public void Post_Poco_As_Json_Success()
    {
        var bin = new HttpBin();

        var car = new Car();
        car.Name = "world";

        var response = bin.Post(car, MediaType.json);

        Assert.IsTrue(response.Data.Contains("world"));
    }

    [TestMethod]
    public void Post_HttpRequestMessage_Success()
    {
    }

    [TestMethod]
    public void Post_ByteArrayContent_Success()
    {
    }


    [TestMethod]
    public void Post_StringContent_Success()
    {
    }

    [TestMethod]
    public void Post_Grapql_Success()
    {
        var client = new Client(throwOnUnsuccessful: false);

        var data = Assemblies.GetEmbeddedResource("ClientTests", "_PostGraphqlData.txt");

        var headers = new Dictionary<string, string>
        {
            {"key", "123" },
            {"clientid", "abcd" },
            {"Content-Type", "application/json"},
            { "Accept", "application/json"},
            { "Authorization", "Bearer eySomething...." }
        };

        var url = "https://graphql.server.com/graphql";

        var r = client.Post<string>(url, data, headers: headers);

        Assert.IsTrue(r != null);
    }
}
