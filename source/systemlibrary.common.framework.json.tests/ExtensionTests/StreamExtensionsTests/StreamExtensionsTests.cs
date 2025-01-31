using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.Extensions;
using SystemLibrary.Common.Framework.Tests;

namespace SystemLibrary.Common.Framework;

[TestClass]
public class StreamExtensionsTests : BaseTest
{
    [TestMethod]
    public void JsonAsync_Generates_Hash()
    {
        var data = Assemblies.GetEmbeddedResource("_Assets/employee.json");

        using (MemoryStream memory = new MemoryStream())
        {
            using (var writer = new StreamWriter(memory))
            {
                writer.Write(data);
                writer.Flush();
                memory.Seek(0, SeekOrigin.Begin);

                var employee = memory.JsonAsync<Employee>().GetAwaiter().GetResult();

                Assert.IsTrue(employee.BirthDate > DateTime.MinValue);
                Assert.IsTrue(employee.Invoices.Count > 0);
            }
        }
    }
}
