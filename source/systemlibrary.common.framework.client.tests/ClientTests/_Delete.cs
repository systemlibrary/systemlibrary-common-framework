using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SystemLibrary.Common.Framework.App.Tests;

partial class ClientTests
{
    [TestMethod]
    public void Delete_Json_Success()
    {
        var bin = new HttpBin();

        var response = bin.Delete("{ \"hello\": \"world\"}", MediaType.json);

        Assert.IsTrue(response.Data.Contains(": \"world\""));
    }
}
