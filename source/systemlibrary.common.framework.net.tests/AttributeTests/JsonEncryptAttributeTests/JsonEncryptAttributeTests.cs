using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.Extensions;
using SystemLibrary.Common.Framework.Tests;

namespace SystemLibrary.Common.Framework;

[TestClass]
public class JsonEncryptAttributeTests : BaseTest
{
    [TestMethod]
    public void Json_Decrypted_On_Read_And_Encrypted_Again_On_Write()
    {
        var data = Assemblies.GetEmbeddedResource("_Assets/jsonEncryptAttribute.json");

        var model = data.Json<JsonEncryptAttributeModel>();

        Assert.IsTrue(model.Age == 39);

        Assert.IsTrue(model.Password == "Hello world");
        Assert.IsTrue(model.Password2 == "Hello world");

        var json = model.Json();

        Assert.IsFalse(json.Contains("Hello world"));
        Assert.IsTrue(json.Length > 160);
    }
}
