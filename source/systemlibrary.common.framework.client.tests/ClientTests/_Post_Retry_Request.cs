using System;
using System.Net.Http;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SystemLibrary.Common.Framework.App.Tests;

partial class ClientTests
{
    [TestMethod]
    public void Post_Retry_Request_Fails()
    {
        try
        {
            var bin = new HttpBin(true);

            var response = bin.Post_Retry_Request_Against_Firewall();

            Assert.IsTrue(false, "Should throw");
        }
        catch (HttpRequestException)
        {
            Assert.IsTrue(true);
        }
        catch (Exception ex)
        {
            Assert.IsTrue(false, "Exception thrown HttpRequestException: " + ex);
        }
    }

    [TestMethod]
    public void Post_Retry_Request_Fails_Retrying_Skipped()
    {
        try
        {
            var bin = new HttpBin(false);

            var response = bin.Post_Retry_Request_Against_Firewall();

            Assert.IsTrue(false, "Should throw");
        }
        catch (HttpRequestException)
        {
            Assert.IsTrue(true);
        }
        catch (Exception ex)
        {
            Assert.IsTrue(false, "Exception thrown HttpRequestException: " + ex);
        }
    }
}
