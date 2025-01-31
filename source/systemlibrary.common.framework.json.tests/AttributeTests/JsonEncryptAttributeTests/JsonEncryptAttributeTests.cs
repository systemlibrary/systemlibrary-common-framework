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

        Assert.IsTrue(model.Age == 39, "Model age: " + model.Age);

        Assert.IsTrue(model.Password == "Hello world", "Password " + model.Password);
        Assert.IsTrue(model.Password2 == "Hello world");

        var json = model.Json();

        Assert.IsFalse(json.Contains("Hello world"), "Json: " + json);
        Assert.IsTrue(json.Length > 160, "Too short, spaces are most likely not printed, missing appSettings.json at root? Json: " + json);
    }
}
