using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SystemLibrary.Common.Framework.App.Tests;

partial class ClientTests
{
    [TestMethod]
    public void Post_Class_As_wwwUrlEncoded_Success()
    {
        var client = new Client();

        var data = new HttpBinFormData();

        var url = "https://httpbin.org/post";

        var response = client.Post(url, data, MediaType.xwwwformUrlEncoded, deserialize: (responseText) => responseText.JsonPartial<HttpBinFormData>("form"));

        Assert.IsTrue(response.IsSuccess, "Not success " + response.Message);

        Assert.IsTrue(response.Data.comments == data.comments, "comments invalid");
        Assert.IsTrue(response.Data.custemail == data.custemail, "custemail invalid");
        Assert.IsTrue(response.Data.delivery == data.delivery, "delivery invalid");
    }

    class HttpBinFormData
    {
        public string comments { get; set; } = "Hello world";
        public string custemail { get; set; } = "test@systemlibrary.com";
        public string custname { get; set; } = "Aa bb";
        public string custtel { get; set; } = "1234567";
        public string delivery { get; set; } = "18:00";
        public string size { get; set; } = "medium";
        public string topping { get; set; } = "bacon";
    }
}

