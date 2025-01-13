using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.Extensions;
using SystemLibrary.Common.Framework.Net.Tests;

namespace SystemLibrary.Common.Framework;

[TestClass]
public class JsonCompressAttributeTests : BaseTest
{
    [TestMethod]
    public void Json_Decompressed_On_Read_And_Compressed_Again_On_Write()
    {
        var expected = new JsonCompressAttributeModel();
        expected.Id = 1;
        expected.Id2 = 777;
        expected.ID3 = 3;
        expected.id4 = 4;
        expected.id7 = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAABBBB?AAAAAAAAAAAAAAAAAAAAAAAAAAAAABBBB?AAAAAAAAAAAAAAAAAAAAAAAAAAAAABBBB?AAAAAAAAAAAAAAAAAAAAAAAAAAAAABBBB?AAAAAAAAAAAAAAAAAAAAAAAAAAAAABBBB?AAAAAAAAAAAAAAAAAAAAAAAAAAAAABBBB?AAAAAAAAAAAAAAAAAAAAAAAAAAAAABBBB?";
        expected.id8 = "abcdefghijklmnopqrstuvwXYZ0123?!__.:.__!";
        expected.id9 = 123456789123456;

        var data = Assemblies.GetEmbeddedResource("_Assets/jsonCompressAttribute.json");

        var model = data.Json<JsonCompressAttributeModel>();

        Assert.IsTrue(model.Id == expected.Id, "id");
        Assert.IsTrue(model.Id2 == expected.Id2, "id2 " + model.Id2 + " " + expected.Id2);
        Assert.IsTrue(model.ID3 == expected.ID3, "id3");
        Assert.IsTrue(model.id4 == expected.id4, "id4");
        Assert.IsTrue(model.id7 == expected.id7, "id7");
        Assert.IsTrue(model.id8 == expected.id8, "id8");
        Assert.IsTrue(model.id9 == expected.id9, "id9");

        Assert.IsTrue(model.id9 == 123456789123456);

        var json = model.Json();

        Assert.IsTrue(!json.Contains("123456789123456"), "Invalid, the long exists in the string json");

    }
}
