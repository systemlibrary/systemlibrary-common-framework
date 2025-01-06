using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SystemLibrary.Common.Framework;

partial class StringExtensionsTests 
{
    [TestMethod]
    public void Obfuscate_Null_Success()
    {
        var data = (string)null;

        var result = data.Obfuscate();
        Assert.IsTrue(result == null);

        result = result.Deobfuscate();
        Assert.IsTrue(result == data, "Failed deobfuscating");
    }

    public void Obfuscate_Empty_Success()
    {
        var data = "";

        var result = data.Obfuscate();
        Assert.IsTrue(result == "");

        result = result.Deobfuscate();
        Assert.IsTrue(result == "", "Failed deobfuscating");
    }

    public void Obfuscate_Text_Success()
    {
        var data = "Hello world";

        var result = data.Obfuscate();
        Assert.IsTrue(result.Length == data.Length && result != data);

        result = result.Deobfuscate();
        Assert.IsTrue(result == data, "Failed deobfuscating");
    }

    public void Obfuscate_Norwegian_And_Strange_Text_Success()
    {
        var data = "A lot of various characters ÆØÅæøå ABCDEFGHIJKLMONPQRSTUVXYZ: 1234567890,.-_?\"*|!#¤%&/()=?…±òü±²³";

        var result = data.Obfuscate();
        Assert.IsTrue(result.Length == data.Length && result != data);

        result = result.Deobfuscate();
        Assert.IsTrue(result == data, "Failed deobfuscating");
    }

    public void Obfuscate_With_Salt_Success()
    {
        var data = "High Salt A lot of various with high salt ÆØÅæøå ABCDEFGHIJKLMONPQRSTUVXYZ: 1234567890,.-_?\"*|!#¤%&/()=?…±òü±²³";
        
        var result = data.Obfuscate(15000);
        Assert.IsTrue(result.Length == data.Length && result != data);

        result = result.Deobfuscate(15000);
        Assert.IsTrue(result == data, "Failed deobfuscating");
    }

    public void Obfuscate_With_MaxSalt_Success()
    {
        var data = "High Salt A lot of various with high salt ÆØÅæøå ABCDEFGHIJKLMONPQRSTUVXYZ: 1234567890,.-_?\"*|!#¤%&/()=?…±òü±²³";

        var result = data.Obfuscate(int.MaxValue);
        Assert.IsTrue(result.Length == data.Length && result != data);

        result = result.Deobfuscate(int.MaxValue);
        Assert.IsTrue(result == data, "Failed deobfuscating");
    }

    public void Obfuscate_With_Negative_Salt_Throws()
    {
        var data = "Hello world";

        try
        {
            var result = data.Obfuscate(-1);
            Assert.IsTrue(false, "Obfuscate with negative salt should throw");
        }
        catch
        {
            Assert.IsTrue(true);
        }
    }

    public void Obfuscate_Multiple_In_A_Row_Success()
    {
        var data = "Hello world";
        var result = data.Obfuscate(0);
        var result2 = data.Obfuscate(1);
        var result3 = data.Obfuscate(100);
        var result4 = data.Obfuscate(1000);

        Assert.IsTrue(result != result2 && result.Length == result2.Length, "Result 2");
        Assert.IsTrue(result != result3 && result.Length == result3.Length, "Result 3");
        Assert.IsTrue(result != result4 && result.Length == result4.Length, "Result 4");
    }
}