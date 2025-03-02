using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SystemLibrary.Common.Framework.App.Tests;

[TestClass]
public class ApiTest
{
    const string LocalApiUrl = "http://localhost/graphql";

    const string LocalApiKeyName = "api-key";
    const string LocalApiKey = "";

    const string LocalApiTokenName = "token";
    const string LocalApiToken = "";

    const string LocalClientId = "app";

    [TestMethod]
    public void Target_Local_Api_With_String_As_Json_Data_Success()
    {
        var client = new Client(20000, true, true);

        var headers = new Dictionary<string, string>()
        {
            { LocalApiKeyName , LocalApiKey},
            {"clientid" , LocalClientId },
            { "Accept" , "application/json" },
            { LocalApiTokenName, LocalApiToken}
        };

        var data = "{\"query\":\"query Query { field0: countries{name countryCode} }\",\"variables\":{}}";

        if (LocalApiKey.IsNot()) return;

        var result = client.Post<string>(LocalApiUrl, data, MediaType.json, headers, 20000);

        try
        {
            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.Data.Contains("Iceland"));
        }
        catch
        {
            throw;
        }
    }

    [TestMethod]
    public void Target_Local_Api_With_Bytes_As_Json_Data_Success()
    {
        var client = new Client(20000, true, true);

        var headers = new Dictionary<string, string>()
        {
            { LocalApiKeyName , LocalApiKey},
            {"clientid" , LocalClientId },
            { "Accept" , "application/json" },
            { LocalApiTokenName, LocalApiToken}
        };

        var data = "{\"query\":\"query Query { field0: countries{name countryCode} }\",\"variables\":{}}";

        if (LocalApiKey.IsNot()) return;

        var result = client.Post<string>(LocalApiUrl, data.GetBytes(), MediaType.json, headers, 20000);

        try
        {
            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.Data.Contains("Iceland"));
        }
        catch
        {
            throw;
        }
    }
}
