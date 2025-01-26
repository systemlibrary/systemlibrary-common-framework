using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.App;

namespace SystemLibrary.Common.Framework.Tests;

[TestClass]
public class ApiTestUserOrigin : BaseTest
{
    public ApiTestUserOrigin()
    {
        this.AssemblyPart = typeof(ApiTestUserOrigin).Assembly;
    }

    [TestMethod]
    public void GetPin_Returns_403_Not_Authorized()
    {
        var responseText = GetResponse("/userAgent/getPin/");

        Assert.IsTrue(responseText.Contains("403"), "No 403 " + responseText);
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

[UserAgentFilter(match: "He.l.lo-User-Agent;(SomeOS)")]
public class UserAgentApiController : BaseApiController
{
    [HttpGet]
    [Route("/userAgent/getPin/")]
    public ActionResult GetPin() => Ok();
}
