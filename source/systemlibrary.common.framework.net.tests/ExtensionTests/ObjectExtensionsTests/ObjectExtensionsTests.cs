using System.Text.Json;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.Extensions;
using SystemLibrary.Common.Framework.Tests;

namespace SystemLibrary.Common.Framework;

[TestClass]
public class ObjectExtensionsTests : BaseTest
{
    [TestMethod]
    public void AsEnumArray_Index_Numbers_To_Enum()
    {
        var integers = new object[] { 1, 2, 3, 40 };

        var colors = integers.AsEnumArray<BackgroundColor>();

        Assert.IsTrue(colors.Length == integers.Length, colors.Length + "");
        Assert.IsTrue(colors[3] == BackgroundColor.yellow, colors[3].ToString());
    }

    [TestMethod]
    public void AsEnumArray_Supports_EnumValue_As_String()
    {
        var texts = new object[] { "Red", "GREEN!", "100" };

        var colors = texts.AsEnumArray<BackgroundColor>();

        Assert.IsTrue(colors.Length == texts.Length);
        Assert.IsTrue(colors[1] == BackgroundColor.Green);
        Assert.IsTrue(colors[2] == BackgroundColor.Blue);
    }

    [TestMethod]
    public void AsEnumArray_Keys_To_Enum()
    {
        var texts = new string[] { "Red", "Green", "Blue" };

        var colors = texts.AsEnumArray<BackgroundColor>();

        Assert.IsTrue(colors.Length == texts.Length);
        Assert.IsTrue(colors[1] == BackgroundColor.Green);
    }
}