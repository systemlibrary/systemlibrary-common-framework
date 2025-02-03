using System.Runtime.CompilerServices;
using System.Text.Json;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.Extensions;
using SystemLibrary.Common.Framework.Tests;

namespace SystemLibrary.Common.Framework;

[TestClass]
public partial class StringExtensionsTests : BaseTest
{
    [TestMethod]
    public void IsJson_Success()
    {
        var data = "{ \"firstName\": \"Hello\", \"age\": 10 }";
        Assert.IsTrue(data.IsJson(), "1");

        data = Assemblies.GetEmbeddedResource("_Assets/employee.json");
        Assert.IsTrue(data.IsJson(), "2");

        data = Assemblies.GetEmbeddedResource("_Assets/employees.json");
        Assert.IsTrue(data.IsJson(), "3");

        data = Assemblies.GetEmbeddedResource("_Assets/response.json");
        Assert.IsTrue(data.IsJson(), "4");

        data = Assemblies.GetEmbeddedResource("_Assets/json-with-new-line.json");
        Assert.IsTrue(data.IsJson(), "5");
    }
    
    [TestMethod]
    public void Json_Partial_Text_To_Employee()
    {
        var data = "{ \"firstName\": \"Hello World\", \"age\": 123 }";

        var user = data.Json<Employee>();

        Assert.IsTrue(user.Age == 123 && user.FirstName == "Hello World");
    }

    [TestMethod]
    public void Json_Case_Sensitive_Does_Not_Match_Age_Success()
    {
        var data = "{ \"FirstName\": \"Hello World\", \"age\": 123 }";

        var options = new JsonSerializerOptions()
        {
            AllowTrailingCommas = true,
            PropertyNameCaseInsensitive = false
        };
        var user = data.Json<Employee>(options);

        Assert.IsTrue(user.FirstName == "Hello World");
        Assert.IsTrue(user.Age == 0);
    }

    [TestMethod]
    public void Json_Blacklisted_Types_Does_Not_Throw()
    {
        var model = new BlacklistedJsonPropertyTypes();

        model.Exception = new Exception("Hello world");
        model.RuntimeWrappedException = new RuntimeWrappedException(new Exception("Hello world"));

        model.IDictionaryTypeString = new Dictionary<int, string>();
        model.IDictionaryTypeString.Add(typeof(SystemType).GetHashCode(), "Hello world");

        var json = model.Json();

        Assert.IsTrue(json.Contains("\"RuntimeWrappedException\": null,"));
    }
}