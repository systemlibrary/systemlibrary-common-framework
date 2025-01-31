using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework.App;

[TestClass]
public class IPrincipalExtensionsTests
{
    [TestMethod]
    public void IsInRole_Success()
    {
        var principal = Fakes.GetPrincipal();

        var isInRole = principal.IsInAnyRole("admin", "Admin", "Administrator", "Administrators");

        Assert.IsTrue(isInRole);
    }

    [TestMethod]
    public void IsInRole_False()
    {
        var principal = Fakes.GetPrincipal();

        var isInRole = principal.IsInAnyRole("Web", "Admin");

        Assert.IsFalse(isInRole);
    }

    [TestMethod]
    public void IsInRole_False_WhenNull()
    {
        var principal = Fakes.GetPrincipal();

        var role = (string)null;

        var isInRole = principal.IsInAnyRole(role);

        Assert.IsFalse(isInRole);
    }
}
