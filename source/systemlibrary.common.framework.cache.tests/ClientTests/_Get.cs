using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework.App.Tests;

partial class ClientTests
{
    [TestMethod]
    public void Get_Success()
    {
        var bin = new HttpBin();
        var response = bin.Get();

        Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.OK);
        Assert.IsTrue(response.Data.Contains("httpbin.org"));
    }

    [TestMethod]
    public void Get_Client_Response_Data_Serialized_Success()
    {
        var bin = new HttpBin();
        var response = bin.Get();

        var clientResponse = response as ClientResponse;

        Assert.IsTrue(clientResponse != null);
        Assert.IsTrue(clientResponse.Data != null, "Data is null, the explicit Data impl. of the interface has changed");
        Assert.IsTrue(clientResponse.Data.Json().Length > 10, "Json too short");
    }

    [TestMethod]
    public void Get_Client_Response_Serialize_Not_Printing_Interface_Data()
    {
        var bin = new HttpBin();
        var response = bin.Get();

        var responseJson = response.Json();

        var clientResponse = response as ClientResponse;

        var json = clientResponse.Json();

        Assert.IsTrue(!responseJson.Contains("IClientResponse"), "IClientResponse is part of responseJson");

        Assert.IsTrue(!json.Contains("IClientResponse"), "IClientResponse is part of json");
    }
}
