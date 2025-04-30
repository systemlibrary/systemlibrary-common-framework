using System.Text.Json;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.App;
using SystemLibrary.Common.Framework.Tests;

namespace SystemLibrary.Common.Framework;

[TestClass]
public class ChatGPTTests : BaseTest
{
    Client _client;
    const int DefaultTimeout = 10000;

    [TestInitialize]
    public void Setup()
    {
        _client = new Client();
    }

    [TestMethod]
    public void Get_Returns_Deserialized_TypicodeModel()
    {
        var response = _client.Get<TypicodeModel>("https://jsonplaceholder.typicode.com/posts/1");

        IsOk(response.Data.Id == 1, "Id is not 1");
    }

    [TestMethod]
    public void Get_Returns_String()
    {
        var response = _client.Get<string>("https://httpbin.org/get", ContentType.text);

        IsOk(response.Data.Contains("url"));
    }

    [TestMethod]
    public void Get_Returns_Bytes_Of_Jpg()
    {
        var response = _client.Get<byte[]>("https://picsum.photos/150", ContentType.jpeg);

        Assert.IsTrue(response.Data.Length > 0);
    }

    [TestMethod]
    public void Get_NoPayload_DefaultsToJson()
    {
        var result = _client.Get<string>("https://httpbin.org/get");

        Assert.IsTrue(result.IsSuccess);
    }

    [TestMethod]
    public void Get_AnonymousPayload_UsesJson()
    {
        var payload = new { Name = "John Kusack", Age = 30 };

        var result = _client.Get<string>("https://postman-echo.com/get", payload: payload);

        Assert.IsTrue(result.Data.Contains("John Kusack"));
    }

    [TestMethod]
    public void Get_FormPayload_UsesFormEncoding()
    {
        var payload = new Dictionary<string, string> { { "key", "value" } };

        var result = _client.Get<string>("https://postman-echo.com/get", ContentType.xwwwformUrlEncoded, payload: payload);

        Assert.IsTrue(result.Data.Contains("value"));
    }

    [TestMethod]
    public void Post_FormPayload_UsesFormEncoding()
    {
        var payload = new Dictionary<string, string> { { "key", "value" } };

        var result = _client.Post<string>("https://postman-echo.com/post", payload, ContentType.xwwwformUrlEncoded);

        Assert.IsTrue(result.Data.Contains("value"));
    }

    [TestMethod]
    public void Put_FormPayload_UsesFormEncoding()
    {
        var payload = new Dictionary<string, string> { { "key", "value" } };

        var result = _client.Put<string>("https://postman-echo.com/put", payload, ContentType.xwwwformUrlEncoded);

        Assert.IsTrue(result.Data.Contains("value"));
    }


    [TestMethod]
    public void Get_WithHeaders()
    {
        var headers = new Dictionary<string, string> { { "Authorization", "Bearer token" } };

        var result = _client.Get<string>("https://httpbin.org/get", ContentType.Auto, headers);

        Assert.IsTrue(result.IsSuccess);
    }

    [TestMethod]
    public void Get_WithTimeout()
    {
        var result = _client.Get<string>("https://httpbin.org/get", ContentType.Auto, null, 5000);

        Assert.IsTrue(result.IsSuccess);
    }

    [TestMethod]
    public void Get_CustomDeserializer()
    {
        var invoked = false;
        Func<string, dynamic> deserialize = json =>
        {
            invoked = true;
            return JsonSerializer.Deserialize<dynamic>(json);
        };

        var result = _client.Get<dynamic>("https://httpbin.org/get", ContentType.Auto, null, DefaultTimeout, null, null, default, deserialize);

        Assert.IsTrue(invoked, "Deserialize not invoked");
        Assert.IsTrue(result.IsSuccess);
    }

    [TestMethod]
    public void Get_Custom_Deserializer_Only()
    {
        var invoked = false;
        Func<string, dynamic> deserialize = json =>
        {
            invoked = true;
            return JsonSerializer.Deserialize<dynamic>(json);
        };

        var result = _client.Get<dynamic>("https://httpbin.org/get", deserialize: deserialize);

        Assert.IsTrue(invoked, "Deserialize not invoked");
        Assert.IsTrue(result.IsSuccess);
    }


    [TestMethod]
    public async Task Get_WithBytePayload_ReturnsExpectedResponse()
    {
        var url = "https://postman-echo.com/post";

        var payload = new byte[] { 1, 2, 3, 4 };
        var headers = new Dictionary<string, string> { { "Authorization", "Bearer token" } };
        var cancellationToken = new CancellationTokenSource(5000).Token;

        var response = await _client.PostAsync<string>(url, payload, ContentType.Auto, headers, 5000, default, cancellationToken);

        Assert.IsTrue(response.IsSuccess);
    }
}
