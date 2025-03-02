using System;
using System.Threading;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.App.Extensions;

namespace SystemLibrary.Common.Framework.App.Tests;

partial class ClientTests
{
    [TestMethod]
    public void Get_Cancel_Callee_Exception_Without_Retrying_Success()
    {
        var ms = Clock.Measure(() =>
        {
            try
            {
                var bin = new HttpBin(false);

                CancellationTokenSource tokenSource = new CancellationTokenSource();

                tokenSource.CancelAfter(75);

                var response = bin.Get_Retry_Request_Against_Firewall(tokenSource.Token);

                throw new Exception(nameof(bin.Get_Retry_Request_Against_Firewall) + " should throw CalleeCancelledRequestException");
            }
            catch (CalleeCancelledRequestException)
            {
                Assert.IsTrue(true);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(false, "CalleeCancelledRequestException should be thrown: " + ex.Message + " " + ex.GetType().Name);
            }
        });

        Assert.IsTrue(ms < 380, "Too long, it did a retry when it shouldnt");
    }

    [TestMethod]
    public void Get_Cancel_Callee_Exception_With_Retrying_Success()
    {
        try
        {
            var bin = new HttpBin(true);

            CancellationTokenSource tokenSource = new CancellationTokenSource();

            tokenSource.CancelAfter(100);

            var response = bin.Get_Retry_Request_Against_Firewall(tokenSource.Token);

            throw new Exception(nameof(bin.Get_Retry_Request_Against_Firewall) + " should throw CalleeCancelledRequestException");
        }
        catch (CalleeCancelledRequestException)
        {
            Assert.IsTrue(true);
        }
        catch (Exception ex)
        {
            Assert.IsTrue(false, "CalleeCancelledRequestException should be thrown: " + ex.Message + " " + ex.GetType().Name);
        }
    }
}
