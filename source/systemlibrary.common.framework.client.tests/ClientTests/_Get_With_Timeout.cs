using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SystemLibrary.Common.Framework.App.Tests;

partial class ClientTests
{
    [TestMethod]
    public void Get_With_Large_Timeout_Success()
    {
        var bin = new HttpBin();

        var response = bin.GetWithTimeout(9222);

        Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.OK);
        Assert.IsTrue(response.Data.Contains("httpbin.org"));
    }

    [TestMethod]
    public void Get_With_Short_Timeout_Fails()
    {
        var bin = new HttpBin();
        int timeout = 123;

        try
        {
            var response = bin.GetWithTimeout(timeout);

            Assert.IsTrue(false, "Should throw HttpRequestException");
        }
        catch (HttpRequestException ex)
        {
            Assert.IsTrue(ex?.Message?.Contains(timeout.ToString() + "ms") == true, ex.Message);
        }
        catch (Exception ex)
        {
            Assert.IsTrue(false, ex.ToString());
        }
    }

    [TestMethod]
    public void Get_With_Short_Timeout_Fails_And_Retry_Is_Success()
    {
        var bin = new HttpBin(true);
        int timeout = 123;
        
        try
        {
            var response = bin.GetWithTimeout(timeout);
        }
        catch (Exception ex)
        {
            Assert.IsTrue(false, "Times out first, a retry requests should get 200 OK without throwing, retry timeout must be larger than 3s + some. Should not have thrown ex: " + ex.ToString());
        }
    }

    [TestMethod]
    public void Get_With_Short_Timeout_Fails_And_Retry_Times_Out()
    {
        var bin = new HttpBin(true);
        int timeout = 123;

        try
        {
            var response = bin.GetWithTimeout(timeout, 7);

            Log.Dump(response);

            Assert.IsTrue(false, "Should throw HttpRequestException as timeout is 123ms and retry timeout is less than the sleep of 11 seconds");
        }
        catch (HttpRequestException)
        {
            Assert.IsTrue(true);
        }
        catch (Exception ex)
        {
            Assert.IsTrue(false, ex.ToString());
        }
    }
}
