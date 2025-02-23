using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SystemLibrary.Common.Framework.App.Tests;

[TestClass]
public partial class ClientTests
{
    [TestMethod]
    public void Measure_Multiple_Get_404_Throws_404()
    {
        var client = new Client();

        for (int i = 0; i < 8; i++)
        {
            Clock.Measure(() =>
            {
                try
                {
                    var response = client.Get<string>("https://httpbin.org/get/?hello=world");
                    Assert.IsTrue(false, "Should throw, appSettings client changed?");
                }
                catch (HttpRequestException ex)
                {
                    Assert.IsTrue(ex.Message.Contains("404 GET"), ex.Message);
                }
            });
        }
    }

    [TestMethod]
    public void Measure_Multiple_Get_Success()
    {
        var client = new Client();

        for (int i = 0; i < 8; i++)
        {
            Clock.Measure(() =>
            {
                var response = client.Get<string>("https://httpbin.org/get?hello=world");

                Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.OK);

                Assert.IsTrue(response.Data.Contains("hello"));
            });
        }
    }
}