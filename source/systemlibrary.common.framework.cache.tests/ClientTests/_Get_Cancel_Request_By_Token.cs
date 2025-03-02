using System.Threading;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SystemLibrary.Common.Framework.App.Tests;

partial class ClientTests
{
    [TestMethod]
    public void Get_Cancel_Request_By_Token_Success()
    {
        var bin = new HttpBin(false);

        CancellationTokenSource tokenSource = new CancellationTokenSource();

        tokenSource.CancelAfter(100);
        try
        {
            var response = bin.GetWithCancellationToken(tokenSource.Token);
        }
        catch (CalleeCancelledRequestException)
        {
            Assert.IsTrue(true);
        }
    }
}
