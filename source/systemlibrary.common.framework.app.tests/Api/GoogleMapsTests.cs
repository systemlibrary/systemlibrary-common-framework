using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SystemLibrary.Common.Framework.App;

[TestClass]
public class GoogleMapsTests : AppBaseTest
{
    [TestMethod]
    public void GetPin_Fails_Invalid_User_Agent_403_Forbidden()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/googleMaps/GetPinOK/");

        request.Headers.TryAddWithoutValidation("User-Agent", "E2dg");

        request.Headers.TryAddWithoutValidation("api-token", "helloworld");

        var response = GetResponse(request);

        Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.Forbidden, "Not forbidden: " + response.StatusCode);
    }

    [TestMethod]
    public void GetPin_Fails_Invalid_Api_Token_403_Forbidden()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/googleMaps/GetPinOK/");

        request.Headers.TryAddWithoutValidation("User-Agent", "Edg");

        request.Headers.TryAddWithoutValidation("api-token", "hello2world");

        var response = GetResponse(request);

        Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.Forbidden, "Not forbidden: " + response.StatusCode);
    }


    [TestMethod]
    public void GetPin_Success_200_OK()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/googleMaps/GetPinOK/");

        request.Headers.TryAddWithoutValidation("User-Agent", "Edg");

        request.Headers.TryAddWithoutValidation("api-token", "helloworld");

        var response = GetResponse(request);

        Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.OK, "Not OK "  + response.StatusCode);

        var text = GetResponseText(response);

        Assert.IsTrue(text == "", "Text was something: " + text);
    }

    [TestMethod]
    public void GetPin_Returns_200_User_Agent_Is_Valid()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/userAgent/getPin/");

        request.Headers.TryAddWithoutValidation("User-Agent", "He.l.lo-User-Agent;(SomeOS)");
        
        var response = GetResponse(request);

        Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.OK);

        var text = GetResponseText(response);

        Assert.IsTrue(!text.Contains("403"), text);
    }

    [TestMethod]
    public void GetPin_Enum_Converted_To_Params_Successfully()
    {
        //public ActionResult GetPin(ProductColor color1, ProductColor color2 = ProductColor.Red, ProductColor color3 = (ProductColor)4) => Ok();
        //public ActionResult GetPin(GeoLocation geoLocation) => Ok();

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/googleMaps/GetPin/?color1=red&color2=blue&color3=yellow&color4=green");

        request.Headers.TryAddWithoutValidation("User-Agent", "Edg");

        request.Headers.TryAddWithoutValidation("api-token", "helloworld");

        var response = GetResponse(request);

        Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.OK, "Not OK " + response.StatusCode);

        var text = GetResponseText(response);

        Assert.IsTrue(text == "", "Text was something: " + text);
    }
}
