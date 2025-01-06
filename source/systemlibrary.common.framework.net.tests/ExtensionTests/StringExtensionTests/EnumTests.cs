using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.Extensions;
using SystemLibrary.Common.Framework.Net.Tests;

namespace SystemLibrary.Common.Framework;

public partial class StringExtensionsTests : BaseTest
{
    [TestMethod]
    public void ToEnum_Success()
    {
        string a0 = null;
        string a00 = "";
        Assert.AreEqual("Red", a0.ToEnum<BackgroundColor>().ToString(), "Null is not red");
        Assert.AreEqual("Red", a00.ToEnum<BackgroundColor>().ToString(), "Empty is not red");

        string a1 = "red";
        string a2 = "Red";
        string a3 = "RED";
        Assert.AreEqual("Red", a1.ToEnum<BackgroundColor>().ToString(), a1);
        Assert.AreEqual("Red", a2.ToEnum<BackgroundColor>().ToString(), a2);
        Assert.AreEqual("Red", a3.ToEnum<BackgroundColor>().ToString(), a3);

        string b1 = "blue";
        string b2 = "BLUE";
        string b3 = "100";
        string b4 = "2";
        Assert.AreEqual("Blue", b1.ToEnum<BackgroundColor>().ToString(), b1);
        Assert.AreEqual("Blue", b2.ToEnum<BackgroundColor>().ToString(), b2);
        Assert.AreEqual("Blue", b3.ToEnum<BackgroundColor>().ToString(), b3);
        Assert.AreEqual("Blue", b4.ToEnum<BackgroundColor>().ToString(), b4);

        int c1 = 100;
        string c2 = "100";
        Assert.AreEqual("Blue", c1.ToString().ToEnum<BackgroundColor>().ToString(), "100 int");
        Assert.AreEqual("Blue", c2.ToEnum<BackgroundColor>().ToString(), "100 str");
    }

    [TestMethod]
    public void Int_To_Enum_Matching_Name_Prefixed_With_Underscore()
    {
        var i997 = 997;
        var i998 = 998;
        var i999 = 999;

        var e997 = i997.ToString().ToEnum<BackgroundColor>();
        var e998 = i998.ToString().ToEnum<BackgroundColor>();
        var e999 = i999.ToString().ToEnum<BackgroundColor>();

        Assert.IsTrue(e997 == BackgroundColor._997, "e997 is not _997, " + e997.ToString());
        Assert.IsTrue(e998 == BackgroundColor._999, "e998 is not _999, " + e998.ToString());
        Assert.IsTrue(e999 == BackgroundColor._999, "e999 is not _999, " + e999.ToString());
    }

    [TestMethod]
    public void ToEnum_Int_Returns_Enum_With_Unscore_Prefixed()
    {
        var i997 = 997;

        var e997 = (BackgroundColor)i997.ToString().ToEnum(typeof(BackgroundColor));

        Assert.IsTrue(e997 == BackgroundColor._997, "e997 is not _997, " + e997.ToString());
    }

    [TestMethod]
    public void ToEnum_Int_Returns_The_Enum_Int_As_The_Int_Does_Not_Match_Enum_Name()
    {
        var i3 = 3;

        var e3 = (BackgroundColor)i3.ToString().ToEnum(typeof(BackgroundColor));

        Assert.IsTrue(e3 == BackgroundColor.Orange, "5 is not Orange " + e3.ToString());
    }

    [TestMethod]
    public void Json_Underscore_name_Returns_Value_Without_Underscore_If_Name_Is_Int()
    {
        BackgroundColor _997 = BackgroundColor._997;
        BackgroundColor _999 = BackgroundColor._999;

        var value7 = _997.ToValue();
        var value9 = _999.ToValue();

        Assert.IsTrue(value7 == "997", "997? " + value7);
        Assert.IsTrue(value9 == "998", "998? " + value9);
    }

    [TestMethod]
    public void ToEnum_Case_Insensitive_Match()
    {
        var data = "blue";
        var res = data.ToEnum<BackgroundColor>();
        Assert.IsTrue(res == BackgroundColor.Blue);

        data = "OrangE";
        res = data.ToEnum<BackgroundColor>();
        Assert.IsTrue(res == BackgroundColor.Orange);

        data = "YELLOW";
        res = data.ToEnum<BackgroundColor>();
        Assert.IsTrue(res == BackgroundColor.yellow);
    }
}