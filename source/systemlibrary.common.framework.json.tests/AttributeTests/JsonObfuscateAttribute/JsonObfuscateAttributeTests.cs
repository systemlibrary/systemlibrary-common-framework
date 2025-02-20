using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.Extensions;
using SystemLibrary.Common.Framework.Tests;

namespace SystemLibrary.Common.Framework;

[TestClass]
public class JsonObfuscateAttributeTests : BaseTest
{
    [TestMethod]
    public void Json_Deobfuscate_On_Read_And_Obfuscate_Again_On_Write()
    {
        var expected = new JsonObfuscateAttributeModel();
        expected.Id = 1;
        expected.Id2 = 777;
        expected.ID3 = 3;
        expected.id4 = 4;
        expected.id7 = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAABBBB?AAAAAAAAAAAAAAAAAAAAAAAAAAAAABBBB?AAAAAAAAAAAAAAAAAAAAAAAAAAAAABBBB?AAAAAAAAAAAAAAAAAAAAAAAAAAAAABBBB?AAAAAAAAAAAAAAAAAAAAAAAAAAAAABBBB?AAAAAAAAAAAAAAAAAAAAAAAAAAAAABBBB?AAAAAAAAAAAAAAAAAAAAAAAAAAAAABBBB?";
        expected.id8 = "abcdefghijklmnopqrstuvwXYZ0123?!__.:.__!";
        expected.id9 = 123456789123456;

        var data = Assemblies.GetEmbeddedResource("_Assets/jsonObfuscateAttribute.json");

        FrameworkConfigInstance.Current.Json.JsonSecureAttributesEnabled = true;

        var model = data.Json<JsonObfuscateAttributeModel>();

        Assert.IsTrue(model.Id == expected.Id, "id");
        Assert.IsTrue(model.Id2 == expected.Id2, "id2 " + model.Id2 + " " + expected.Id2);
        Assert.IsTrue(model.ID3 == expected.ID3, "id3");
        Assert.IsTrue(model.id4 == expected.id4, "id4");
        Assert.IsTrue(model.id7 == expected.id7, "id7");
        Assert.IsTrue(model.id8 == expected.id8, "id8");
        Assert.IsTrue(model.id9 == expected.id9, "id9");

        Assert.IsTrue(model.id9 == 123456789123456);

        FrameworkConfigInstance.Current.Json.JsonSecureAttributesEnabled = true;

        var json = model.Json();

        Assert.IsTrue(!json.Contains("123456789123456"), "Invalid, the long exists in the string json");
        Assert.IsTrue(json.Contains("LLLLLLLLLLLLLLLLLLLLLLLLLLLLL"), "Invalid, id4 LLLLLLLLLLLLLLLLLLLLLLLLLLLLL is invalid");
    }

    [TestMethod]
    public void Json_Secure_Attributes_Disable_Does_Not_Deobfuscate_Nor_Obfuscate()
    {
        var expected = new JsonObfuscateAttributeModelIgnored();
        expected.Id = 1;
        expected.Id2 = 777;
        expected.ID3 = 3;
        expected.id4 = 4;
        expected.id7 = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAABBBB?AAAAAAAAAAAAAAAAAAAAAAAAAAAAABBBB?AAAAAAAAAAAAAAAAAAAAAAAAAAAAABBBB?AAAAAAAAAAAAAAAAAAAAAAAAAAAAABBBB?AAAAAAAAAAAAAAAAAAAAAAAAAAAAABBBB?AAAAAAAAAAAAAAAAAAAAAAAAAAAAABBBB?AAAAAAAAAAAAAAAAAAAAAAAAAAAAABBBB?";
        expected.id8 = "abcdefghijklmnopqrstuvwXYZ0123?!__.:.__!";
        expected.id9 = 123456789123456;

        var data = Assemblies.GetEmbeddedResource("_Assets/jsonObfuscateAttributeIgnored.json");

        FrameworkConfigInstance.Current.Json.JsonSecureAttributesEnabled = false;
        var model = data.Json<JsonObfuscateAttributeModelIgnored>();
        FrameworkConfigInstance.Current.Json.JsonSecureAttributesEnabled = true;

        Assert.IsTrue(model.Id == expected.Id, "id");
        Assert.IsTrue(model.Id2 == expected.Id2, "id2 " + model.Id2 + " " + expected.Id2);
        Assert.IsTrue(model.ID3 == expected.ID3, "id3");
        Assert.IsTrue(model.id4 == expected.id4, "id4");
        Assert.IsTrue(model.id7 == expected.id7, "id7");
        Assert.IsTrue(model.id8 == expected.id8, "id8");
        Assert.IsTrue(model.id9 == expected.id9, "id9 ");
        Assert.IsTrue(model.id9 == 123456789123456);

        FrameworkConfigInstance.Current.Json.JsonSecureAttributesEnabled = false;
        var json = model.Json();
        FrameworkConfigInstance.Current.Json.JsonSecureAttributesEnabled = true;

        Assert.IsTrue(json.Contains("123456789123456"), "Invalid, the obfuscation ran when it should not due to a framework config");
        Assert.IsTrue(!json.Contains("LLLLLLLLLLLLLLLLLLLLLLLLLLLLL"), "Invalid, the obfuscation ran when it should not due to a framework config");
    }
}
