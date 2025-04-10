using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.Tests;

namespace SystemLibrary.Common.Framework.App.Tests;

[TestClass]
public partial class MetricTests : BaseTest
{
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