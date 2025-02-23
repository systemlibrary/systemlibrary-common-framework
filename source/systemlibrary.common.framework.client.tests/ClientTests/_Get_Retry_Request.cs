using System;
using System.Net.Http;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SystemLibrary.Common.Framework.App.Tests;

partial class ClientTests
{
    [TestMethod]
    public void Get_Retry_Request_Success()
    {
        try
        {
            var bin = new HttpBin(true);

            var response = bin.Get_Retry_Request_Against_Firewall();

            throw new Exception(nameof(bin.Get_Retry_Request_Against_Firewall) + " should throw exception of type: HttpRequestException");
        }
        catch (HttpRequestException)
        {
            Assert.IsTrue(true);
        }
        catch (Exception ex)
        {
            Assert.IsTrue(false, "HttpRequestException should be thrown: " + ex.Message);
        }
    }
}
