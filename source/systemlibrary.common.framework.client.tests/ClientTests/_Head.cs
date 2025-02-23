using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SystemLibrary.Common.Framework.App.Tests;

public partial class ClientTests
{
    [TestMethod]
    public void Head_Success()
    {
        var bin = new HttpBin();

        var response = bin.Head(MediaType.None);

        Assert.IsTrue(response.IsSuccess);
    }
}
