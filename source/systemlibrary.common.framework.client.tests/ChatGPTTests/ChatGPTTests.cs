using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.App;
using SystemLibrary.Common.Framework.Tests;

namespace SystemLibrary.Common.Framework;

[TestClass]
public class ChatGPTTests : BaseTest
{
    [TestMethod]
    public void Get_Returns_Deserialized_TypicodeModel()
    {
        var client = new Client();

        var response = client.Get<TypicodeModel>("https://jsonplaceholder.typicode.com/posts/1");

        IsOk(response.Data.Id == 1, "Id is not 1");
    }

    [TestMethod]
    public void Get_Returns_String()
    {
        var client = new Client();

        var response = client.Get<string>("https://httpbin.org/get", ContentType.text);

        IsOk(response.Data.Contains("url"));
    }

    [TestMethod]
    public void Get_Returns_Bytes_Of_Jpg()
    {
        var client = new Client();

        var response = client.Get<byte[]>("https://picsum.photos/150", ContentType.jpeg);
        
        Assert.IsTrue(response.Data.Length > 0);
    }
}
