using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.Extensions;
using SystemLibrary.Common.Framework.Net.Tests;

namespace SystemLibrary.Common.Framework;

[TestClass]
public class EnumExtensionsTests : BaseTest
{
    [TestMethod]
    public void IsAny_Returns_Success()
    {
        var red = BackgroundColor.Red;

        var result = red.IsAny(BackgroundColor.Green, BackgroundColor.Blue);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void GetValues_Returns_Values_Based_On_EnumValue_First_Then_Name()
    {
        var values = EnumExtensions<BackgroundColor>.GetValues();

        Assert.IsTrue(values.Count() == 7, values.Count() + "");

        Assert.IsTrue(values.First() == "Red", values.First());

        Assert.IsTrue(values.Skip(1).First() == "GREEN!", values.Skip(1).First());

        Assert.IsTrue(values.Last() == "998");
    }

    [TestMethod]
    public void GetKeys_Returns_All_Enum_Keys()
    {
        var keys = EnumExtensions<BackgroundColor>.GetKeys();
        Assert.IsTrue(keys.Count() == 7);

        Assert.IsTrue(keys.Last() == "_999");
    }

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

    [TestMethod]
    public void ToValue_Enum_Success()
    {
        BackgroundColor a = BackgroundColor.Red;
        BackgroundColor b = BackgroundColor.Green;
        BackgroundColor c = BackgroundColor.Blue;

        Assert.AreEqual(a.ToValue(), "Red", "Red");
        Assert.AreEqual(b.ToValue(), "GREEN!", "GREEN!");
        Assert.AreEqual(c.ToValue(), "100", "100");
    }

    [TestMethod]
    public void GetEnumValue_Success()
    {
        BackgroundColor a = BackgroundColor.Red;
        BackgroundColor b = BackgroundColor.Green;
        BackgroundColor c = BackgroundColor.Blue;

        Assert.AreEqual(a.GetEnumValue(), null, "Red");
        Assert.AreEqual(b.GetEnumValue(), "GREEN!", "GREEN!");
        Assert.AreEqual(c.GetEnumValue(), 100, "100 returns was not an int");
    }

    [TestMethod]
    public void GetEnumText_Success()
    {
        BackgroundColor a = BackgroundColor.Red;
        BackgroundColor b = BackgroundColor.Green;
        BackgroundColor c = BackgroundColor.Blue;

        Assert.AreEqual(a.GetEnumText(), null, "Red");
        Assert.AreEqual(b.GetEnumText(), "GREEN", "GREEN");
        Assert.AreEqual(c.GetEnumText(), "BLUE", "BLUE");
    }

    [TestMethod]
    public void ToText_Success()
    {
        BackgroundColor a = BackgroundColor.Red;
        BackgroundColor b = BackgroundColor.Green;
        BackgroundColor c = BackgroundColor.Blue;

        Assert.AreEqual(a.ToText(), "Red");
        Assert.AreEqual(b.ToText(), "GREEN");
        Assert.AreEqual(c.ToText(), "BLUE");
    }
}
