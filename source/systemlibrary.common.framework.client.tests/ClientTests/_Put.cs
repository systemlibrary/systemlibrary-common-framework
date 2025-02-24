using System.Net.Http;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SystemLibrary.Common.Framework.App.Tests;

partial class ClientTests
{
    [TestMethod]
    public void Put_Plain_Text_Success()
    {
        var bin = new HttpBin();

        var response = bin.Put("Hello world!", MediaType.plain);

        Assert.IsTrue(response.Data.Contains("Hello world!"));
    }

    [TestMethod]
    public void Put_Json_Success()
    {
        var bin = new HttpBin();

        var response = bin.Put("{ \"hello\": \"world\" }", MediaType.plain);

        Assert.IsTrue(response.Data.Contains(": \"world\""));
    }

    [TestMethod]
    public void Put_Poco_As_Json_Success()
    {
        var bin = new HttpBin();

        var employee = Employee.Create();

        var response = bin.Put(employee, MediaType.json);

        Assert.IsTrue(response.Data.Contains(": \"John\""));
    }

    [TestMethod]
    public void Put_Anonymous_Object_As_Json_Success()
    {
        var bin = new HttpBin();

        var response = bin.Put(new { name = "world" }, MediaType.json);

        Assert.IsTrue(response.Data.Contains(": \"world\""));
    }

    [TestMethod]
    public void Put_Dynamic_As_Json_Success()
    {
        var bin = new HttpBin();

        var car = new
        {
            name = "world"
        };
        var response = bin.Put(car, MediaType.json);

        Assert.IsTrue(response.Data.Contains(": \"world\""));
    }

    [TestMethod]
    public void Put_Gone_Throws_Exception_With_StatusCode()
    {
        var client = new Client();
        try
        {
            var res = client.Put<string>("https://httpbin.org/status/410", "", MediaType.plain, null, 5000);

            Assert.IsTrue(false, "Should throw, it should return status 410 GONE: " + res.StatusCode);
        }
        catch (HttpRequestException ex)
        {
            Assert.IsTrue(ex.StatusCode > 0, "Ex do not contain status code " + ex.StatusCode + " " + ex.ToString());
        }
    }
}
