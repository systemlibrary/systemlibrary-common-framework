using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.Extensions;
using SystemLibrary.Common.Framework.Net.Tests;

namespace SystemLibrary.Common.Framework;

[TestClass]
public class StreamExtensionsTests : BaseTest
{
    [TestMethod]
    public void ToMD5Hash_Generates_Hash()
    {
        var data = "hello world";

        using (MemoryStream memory = new MemoryStream())
        {
            using (var writer = new StreamWriter(memory))
            {
                writer.Write(data);
                memory.Seek(0, SeekOrigin.Begin);

                var result = memory.ToMD5Hash();

                Assert.IsTrue(result.Length == 47, "Md5Hash Length: " + result.Length);
            }
        }
    }

    [TestMethod]
    public void ToMD5HashAsync_Generates_Hash()
    {
        var data = "hello world";

        using (MemoryStream memory = new MemoryStream())
        {
            using (var writer = new StreamWriter(memory))
            {
                writer.Write(data);
                memory.Seek(0, SeekOrigin.Begin);

                var result = memory.ToMD5HashAsync()
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult();

                Assert.IsTrue(result.Length == 47, "Md5Hash Length: " + result.Length);
            }
        }
    }

    [TestMethod]
    public void ToSha1Hash_Generates_Hash()
    {
        var data = "hello world";

        using (MemoryStream memory = new MemoryStream())
        {
            using (var writer = new StreamWriter(memory))
            {
                writer.Write(data);
                memory.Seek(0, SeekOrigin.Begin);

                var result = memory.ToSha1Hash();

                Assert.IsTrue(result.Length == 59, "Sha1 Length: " + result.Length);
            }
        }
    }

    [TestMethod]
    public void ToSha1HashAsync_Generates_Hash()
    {
        var data = "hello world";

        using (MemoryStream memory = new MemoryStream())
        {
            using (var writer = new StreamWriter(memory))
            {
                writer.Write(data);
                memory.Seek(0, SeekOrigin.Begin);

                var result = memory.ToSha1HashAsync()
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult();

                Assert.IsTrue(result.Length == 59, "Sha1 Length: " + result.Length);
            }
        }
    }

    [TestMethod]
    public void ToSha256Hash_Generates_Hash()
    {
        var data = "hello world";

        using (MemoryStream memory = new MemoryStream())
        {
            using (var writer = new StreamWriter(memory))
            {
                writer.Write(data);
                memory.Seek(0, SeekOrigin.Begin);

                var result = memory.ToSha256Hash();

                Assert.IsTrue(result.Length == 95, "Sha256 Length: " + result.Length);
            }
        }
    }

    [TestMethod]
    public void ToSha256HashAsync_Generates_Hash()
    {
        var data = "hello world";

        using (MemoryStream memory = new MemoryStream())
        {
            using (var writer = new StreamWriter(memory))
            {
                writer.Write(data);
                memory.Seek(0, SeekOrigin.Begin);

                var result = memory.ToSha256HashAsync()
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult();

                Assert.IsTrue(result.Length == 95, "Sha256 Length: " + result.Length);
            }
        }
    }

    [TestMethod]
    public void JsonAsync_Generates_Hash()
    {
        var data = Assemblies.GetEmbeddedResource("_Files/employee.json");

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
