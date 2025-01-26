using System.Collections.Concurrent;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.Extensions;
using SystemLibrary.Common.Framework.Tests;

namespace SystemLibrary.Common.Framework;

[TestClass]
public partial class ConcurrentDictionaryExtensionsTests : BaseTest
{
    [TestMethod]
    public void Cache_Not_Exists_Ignores_Cache_And_Returns_String()
    {
        var type = typeof(string);

        ConcurrentDictionary<string, string> cache = null;

        var result = cache.Cache(type.Name, () => "Hello world");

        Assert.IsTrue(result == "Hello world");
    }

    [TestMethod]
    public void Cache_Type_As_Int_Returns_String()
    {
        var type = typeof(string);

        ConcurrentDictionary<int, string> cache = new ConcurrentDictionary<int, string>();

        var result = cache.Cache(type, () => "Hello world");

        Assert.IsTrue(result == "Hello world");
    }

    [TestMethod]
    public void Cache_String_Returns_String()
    {
        var type = typeof(string);

        ConcurrentDictionary<string, string> cache = new ConcurrentDictionary<string, string>();

        var result = cache.Cache(type.Name, () => "Hello world");

        Assert.IsTrue(result == "Hello world");
    }

    [TestMethod]
    public void Cache_String_Multiple_Invocations_Returns_String()
    {
        var type = typeof(string);

        ConcurrentDictionary<int, string> cache = new ConcurrentDictionary<int, string>();

        var result = cache.Cache(type, () => "Hello world");
        Assert.IsTrue(result == "Hello world");

        result = cache.Cache(type, () => "Not returned");
        Assert.IsTrue(result == "Hello world");

        result = cache.Cache(type, () => "Not returned");
        Assert.IsTrue(result == "Hello world");

        result = cache.Cache(type, () => "Not returned");
        Assert.IsTrue(result == "Hello world");

        result = cache.Cache(1, () => "1?");
        Assert.IsTrue(result == "1?");
    }

    [TestMethod]
    public void TryGet_StringKey_Multiple_Invocations_Returns_Item()
    {
        var type = typeof(string);

        ConcurrentDictionary<string, string> cache = new ConcurrentDictionary<string, string>();

        var result = cache.Cache(type.Name, () => "Hello world");
        Assert.IsTrue(result == "Hello world");

        result = cache.Cache(type.Name, () => "Not returned");
        Assert.IsTrue(result == "Hello world");

        result = cache.Cache(type.Name, () => "Not returned");
        Assert.IsTrue(result == "Hello world");

        result = cache.Cache(type.Name, () => "Not returned");
        Assert.IsTrue(result == "Hello world");
    }
}
