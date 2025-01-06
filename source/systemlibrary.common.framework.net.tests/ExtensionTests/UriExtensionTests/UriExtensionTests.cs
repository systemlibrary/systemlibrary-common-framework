using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.Extensions;
using SystemLibrary.Common.Framework.Net.Tests;

namespace SystemLibrary.Common.Framework;

[TestClass]
public class UriExtensionTests : BaseTest
{
    [TestMethod]
    public void GetPrimaryDomain_As_Null_Success()
    {
        Uri uri = null;

        var primaryDomain = uri.GetPrimaryDomain();

        Assert.IsTrue(primaryDomain == "");
    }

    [TestMethod]
    public void GetPrimaryDomain_Url_Success()
    {
        var uri = new Uri("http://localhost.no");

        var primaryDomain = uri.GetPrimaryDomain();

        Assert.IsTrue(primaryDomain == "localhost.no");
    }

    [TestMethod]
    public void GetPrimaryDomain_Subdomain_Url_Success()
    {
        var uri = new Uri("http://www.domain1.domain2.localhost.no");

        var primaryDomain = uri.GetPrimaryDomain();

        Assert.IsTrue(primaryDomain == "localhost.no");
    }

    [TestMethod]
    public void GetPrimaryDomain_Without_Language_Part_Success()
    {
        // TODO: This is probably quite stupid... 

        var uri = new Uri("http://localhost");

        var primaryDomain = uri.GetPrimaryDomain();

        Assert.IsTrue(primaryDomain == "localhost.com");
    }

    [TestMethod]
    public void GetPrimaryDomain_Without_Protocol_And_Without_Language_Success()
    {
        // TODO: This is also probably stupid
        var uri = new Uri("system.library", UriKind.Relative);

        var primaryDomain = uri.GetPrimaryDomain();

        Assert.IsTrue(primaryDomain == "library.com");
    }

    [TestMethod]
    public void GetPrimaryDomain_No_Protocol_Valid_Language_Part_Success()
    {
        var uri = new Uri("system.demo", UriKind.Relative);

        var primaryDomain = uri.GetPrimaryDomain();

        Assert.IsTrue(primaryDomain == "system.demo");
    }

    [TestMethod]
    public void GetPrimaryDomain_No_Protocol_Valid_Subdomain_Url_Success()
    {
        var uri = new Uri("system.library.no", UriKind.Relative);

        var primaryDomain = uri.GetPrimaryDomain();

        Assert.IsTrue(primaryDomain == "library.no");
    }
}
