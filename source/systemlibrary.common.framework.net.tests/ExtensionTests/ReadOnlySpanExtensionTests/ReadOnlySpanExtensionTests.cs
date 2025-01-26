using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.Extensions;
using SystemLibrary.Common.Framework.Tests;

namespace SystemLibrary.Common.Framework;

[TestClass]
public class ReadOnlySpanExtensionTests : BaseTest
{
    [TestMethod]
    public void ToBase64_Returns_Data()
    {
        var data = "hello world";
        var datab64 = data.ToBase64();

        var span = data.AsSpan();
        var span64 = span.ToBase64();

        var sub = data.Substring(0);
        var sub64 = sub.ToBase64();

        Assert.IsTrue(datab64 == sub64 && sub64 == span64);
    }

    [TestMethod]
    public void GetBytes_Returns_Data()
    {
        var data = "hello world";
        var bytes = data.GetBytes();

        Assert.IsTrue(bytes.Length == 11);
        Assert.IsTrue(bytes[0] > 0);
    }
}
