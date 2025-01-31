using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SystemLibrary.Common.Framework.App;

[TestClass]
public class IServiceCollectionExtensionsTests
{
    [TestMethod]
    public void Use_Existing_Key_Ring_File_With_Auto_Data_Protection_Policy_False_No_Debug_Message()
    {
        var data = "Hello world";

        var enc = data.Encrypt();

        var dec = enc.Decrypt();

        Assert.IsTrue(dec == data, "Wrong decryption");
    }

    [TestMethod]
    public void Use_Auto_Generated_App_Name_As_Key()
    {
        var data = "Hello world";
        var enc = data.Encrypt();
        var dec = enc.Decrypt();

        Assert.IsTrue(dec == data, "Wrong decryption");
    }
}