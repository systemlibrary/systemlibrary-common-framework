using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.Licensing;
using SystemLibrary.Common.Framework.Tests;

namespace SystemLibrary.Common.Framework.App;

[TestClass]
public class LicenseTierTests : BaseTest
{
    [TestInitialize]
    public void Setup()
    {
        var companyId = "1";
        var companyName = "Demo";

        var license = License.Generate(companyId, companyName, License.Tier.Silver);

        License.BypassEnvironmentCheck = true;
        License.TestLicense = license;
    }

    [TestMethod]
    public void Bronze_Is_Part_Of_Silver_Tier()
    {
        var isValid = License.Bronze();

        IsOk(isValid);
    }

    [TestMethod]
    public void Silver_Is_Part_Of_Silver_Tier()
    {
        var isValid = License.Silver();

        IsOk(isValid);
    }

    [TestMethod]
    public void Gold_Is_Not_Part_Of_Silver_Tier()
    {
        var isValid = License.Gold();

        IsNotOk(isValid);
    }

    [TestMethod]
    public void Pipe_Name_And_Id_Does_IsOk()
    {
        var companyId = "1|2|3";
        var companyName = "System|Library";

        var license = License.Generate(companyId, companyName, License.Tier.Gold);

        License.TestLicense = license;

        IsOk(License.Bronze());
        IsOk(License.Silver());
        IsOk(License.Gold());
        IsOk(License.Bronze());
        IsOk(License.Silver());
        IsOk(License.Gold());
    }

    [TestMethod]
    public void Company_Name_Allows_All_Tiers()
    {
        var companyId = (string)null;
        var companyName = "SystemLibrary";

        var license = License.Generate(companyId, companyName, License.Tier.Gold);

        License.TestLicense = license;

        IsOk(License.Bronze());
        IsOk(License.Silver());
        IsOk(License.Gold());
    }
}