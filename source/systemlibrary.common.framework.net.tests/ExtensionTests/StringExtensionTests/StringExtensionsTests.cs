using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.Tests;

namespace SystemLibrary.Common.Framework;

[TestClass]
public partial class StringExtensionsTests : BaseTest
{
    [TestMethod]
    public void Is_Success()
    {
        string data = null;

        var result = data.Is();
        Assert.IsFalse(result, "Is null");

        data = "";
        result = data.Is();
        Assert.IsFalse(result, "Is empty");

        data = " ";
        result = data.Is();
        Assert.IsFalse(result, "Is space");

        data = "  ";
        result = data.Is();
        Assert.IsTrue(result, "Is 2 spaces");

        data = "A";
        result = data.Is();
        Assert.IsTrue(result, "A");

        data = ".";
        result = data.Is();
        Assert.IsTrue(result, "dot");
    }

    [TestMethod]
    public void IsNot_Success()
    {
        string data = null;

        var result = data.IsNot();
        Assert.IsTrue(result, "Result not null");

        data = "";
        result = data.IsNot();
        Assert.IsTrue(result, "Result not empty");

        data = " ";
        result = data.IsNot();
        Assert.IsTrue(result, "Result not space");

        data = "  ";
        result = data.IsNot();
        Assert.IsFalse(result, "Result not 2 spaces");

        data = "Hello world";
        result = data.IsNot("Hello", "Hello woRld", "Hello world");
        Assert.IsTrue(result, "Result not 'Hello world'");

        data = "abcdHello world12345";
        result = data.IsNot("Hello", "Hello woRld", "Hello world");
        Assert.IsFalse(result, "Result not 'Hello world'");
    }

    [TestMethod]
    public void ToMD5Hash_Success()
    {
        string data = null;
        string result = null;

        result = data.ToMD5Hash();
        Assert.IsTrue(result == null);

        data = "";
        result = data.ToMD5Hash();
        Assert.IsTrue(result == "");

        data = "Hello world";
        result = data.ToMD5Hash();
        Assert.IsTrue(result.Length == 47, "length");

        var data2 = "Hello world";
        var result2 = data2.ToMD5Hash();
        Assert.IsTrue(result == result2, "Did not generate two equal hashes");
    }

    [TestMethod]
    public void ToSha1Hash_Success()
    {
        string data = null;
        string result = null;

        result = data.ToSha1Hash();
        Assert.IsTrue(result == null);

        data = "";
        result = data.ToSha1Hash();
        Assert.IsTrue(result == "");

        data = "Hello world";

        result = data.ToSha1Hash();
        Assert.IsTrue(result.Length == 59, "Sha1 length: " + result.Length);

        var data2 = "Hello world";
        var result2 = data2.ToSha1Hash();
        Assert.IsTrue(result == result2, "Did not generate two equal hashes");
    }

    [TestMethod]
    public void ToSha256Hash_Success()
    {
        string data = null;
        string result = null;

        result = data.ToSha256Hash();
        Assert.IsTrue(result == null);

        data = "";
        result = data.ToSha256Hash();
        Assert.IsTrue(result == "");

        data = "Hello world";

        result = data.ToSha256Hash();
        Assert.IsTrue(result.Length == 95, "Sha256 length: " + result.Length);

        var data2 = "Hello world";
        var result2 = data2.ToSha256Hash();
        Assert.IsTrue(result == result2, "Did not generate two equal hashes");
    }

    [TestMethod]
    public void ToBase64_Success()
    {
        string data = null;
        string result;

        result = data.ToBase64();
        Assert.IsTrue(result == null);
        result = result.FromBase64(Encoding.UTF8);
        Assert.IsTrue(result == data);

        data = "";
        result = data.ToBase64();
        Assert.IsTrue(result == "");
        result = result.FromBase64(Encoding.UTF8);
        Assert.IsTrue(result == data);

        data = "hello world";
        result = data.ToBase64();
        Assert.IsTrue(result.Length > 10 && result.EndsWith("="));
        result = result.FromBase64(Encoding.UTF8);
        Assert.IsTrue(result == data);

        data = "A lot of various characters ÆØÅæøå ABCDEFGHIJKLMONPQRSTUVXYZ: 1234567890,.-_?\"*|!#¤%&/()=?…±òü±²³";
        result = data.ToBase64();
        Assert.IsTrue(result != null);
        result = result.FromBase64(Encoding.UTF8);
        Assert.IsTrue(result == data && result.Length == data.Length);
    }


    [TestMethod]
    public void OrFirstOf_Success()
    {
        var s1 = (string)null;
        var s2 = "";
        var s3 = " ";
        var s4 = "";
        var s5 = " ";
        var s6 = "Hello world";
        var s7 = (string)null;

        var res = s1.OrFirstOf(s2, s3, s4, s5, s6, s7);

        Assert.IsTrue(res == "Hello world");

        var res2 = s2.OrFirstOf(s1, s6, s5);
        Assert.IsTrue(res == "Hello world");

        var res3 = s6.OrFirstOf(s5, s4, s3);
        Assert.IsTrue(res == "Hello world");
    }

    [TestMethod]
    public void MaxLength_Success()
    {
        var data = (string)null;
        var res = data.MaxLength(3);

        Assert.IsTrue(res == "", "Res it not null when passing in null");

        data = "";
        res = data.MaxLength(3);
        Assert.IsTrue(res == "", "Res it not blank when passing in blank");

        data = "1";
        res = data.MaxLength(3);
        Assert.IsTrue(res == "1", "Res is not 1 when passing in '1'");

        data = "123";
        res = data.MaxLength(3);
        Assert.IsTrue(res == "123", "Res is not 123 when passing in '123'");

        data = "1234";
        res = data.MaxLength(3);
        Assert.IsTrue(res == "123", "Res is not 123 when passing in '1234'");

        data = "1234 hello world, long text @.,.-!'æ¨¨æ¨ææ¨%#&=)#&#!(=!/?=/(?=";
        res = data.MaxLength(3);
        Assert.IsTrue(res == "123", "Res is not 123 when passing in a long text with many special chars");

        data = "1234 hello world, long text @.,.-!'æ¨¨æ¨ææ¨%#&=)#&#!(=!/?=/(?=";
        res = data.MaxLength(-1);
        Assert.IsTrue(res == "", "Res is not blank when passing in a long text with many special chars, but max length negative");
    }

    [TestMethod]
    public void IsAny_Success()
    {
        var data = "world";

        var any = new[] { "hello", "world" };

        Assert.IsTrue(data.IsAny(any));
    }

    [TestMethod]
    public void ContainsAny_Success()
    {
        var data = "Hello world, hello world";

        var any = new[] { "abc", "def", "hello" };

        Assert.IsTrue(data.ContainsAny(any));
    }

    [TestMethod]
    public void EndsWithAny_Success()
    {
        var data = "Hello world";

        var any = new[] { "", "data", "world" };

        Assert.IsTrue(data.EndsWithAny(any));
    }

    [TestMethod]
    public void EndsWithAnyCharacter_Success()
    {
        var data = "Hello world";
        var any = "abcdef";

        Assert.IsTrue(data.EndsWithAnyCharacter(any));
    }

    [TestMethod]
    public void EndsWithAny_Case_Insensitive_Success()
    {
        var data = "hello WorLd123!aA";
        var result = data.EndsWithAny(StringComparison.OrdinalIgnoreCase, "helloworld", "something", "another one", "whatever", "world123!aa");
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void StartsWithAny()
    {
        var data = "Hello world 123456";
        var result = false;

        result = data.StartsWithAny("world", "1234", "abc", "ello", "Hello");
        Assert.IsTrue(result, "Does not start with any");

        result = data.StartsWithAny(null);
        Assert.IsTrue(result == false, "Values is null");

        result = data.StartsWithAny("");
        Assert.IsTrue(result == true, "Values is false when Empty");

        result = data.StartsWithAny("H");
        Assert.IsTrue(result, "H");

        result = data.StartsWithAny(StringComparison.OrdinalIgnoreCase, "h");
        Assert.IsTrue(result, "h");
    }

    [TestMethod]
    public void TrimEnd_Success()
    {
        Assert.IsTrue("".TrimEnd("/") == "");

        var data = "/";

        Assert.IsTrue(!data.TrimEnd("/").Contains("/"));
        Assert.IsTrue(!data.TrimEnd("\\").Contains("\\"));

        data = "hello/world/this/is/it/";

        Assert.IsTrue(data.TrimEnd("/").Contains("/"));
        Assert.IsTrue(!data.TrimEnd("/").EndsWith("/"));
    }

    [TestMethod]
    public void GetPrimaryDomain_Success()
    {
        Assert.IsTrue(((string)null).GetPrimaryDomain() == "");
        Assert.IsTrue(" ".GetPrimaryDomain() == "");
        Assert.IsTrue("".GetPrimaryDomain() == "");
        Assert.IsTrue("a".GetPrimaryDomain() == "a.com");
        Assert.IsTrue("abc".GetPrimaryDomain() == "abc.com");
        Assert.IsTrue("abc.com".GetPrimaryDomain() == "abc.com");

        Assert.IsTrue("hello.world".GetPrimaryDomain() == "world.com", "hello.world");
        Assert.IsTrue("hello.world.n".GetPrimaryDomain() == "world.n", "n");

        Assert.IsTrue("hello.world.no".GetPrimaryDomain() == "world.no", ".no");
        Assert.IsTrue("hello.world.com".GetPrimaryDomain() == "world.com", ".com");
        Assert.IsTrue("hello.world.web".GetPrimaryDomain() == "world.web", ".web");
        Assert.IsTrue("hello.world.config".GetPrimaryDomain() == "config.com", "config");

        Assert.IsTrue("http://hello.world.n".GetPrimaryDomain() == "world.n");
        Assert.IsTrue("http://hello.world.no".GetPrimaryDomain() == "world.no");
        Assert.IsTrue("http://hello.world.com".GetPrimaryDomain() == "world.com");
        Assert.IsTrue("http://hello.world.web".GetPrimaryDomain() == "world.web");
        Assert.IsTrue("http://hello.world.config".GetPrimaryDomain() == "config.com");


        Assert.IsTrue("https://hello.world.n".GetPrimaryDomain() == "world.n");
        Assert.IsTrue("https://hello.world.no".GetPrimaryDomain() == "world.no");
        Assert.IsTrue("https://hello.world.com".GetPrimaryDomain() == "world.com");
        Assert.IsTrue("https://hello.world.web".GetPrimaryDomain() == "world.web");
        Assert.IsTrue("https://hello.world.config".GetPrimaryDomain() == "config.com");

        Assert.IsTrue("https://www.hello.world.n".GetPrimaryDomain() == "world.n");
        Assert.IsTrue("https://www.hello.world.no".GetPrimaryDomain() == "world.no");
        Assert.IsTrue("https://www.hello.world.com".GetPrimaryDomain() == "world.com");
        Assert.IsTrue("https://www.hello.world.web".GetPrimaryDomain() == "world.web");
        Assert.IsTrue("https://www.hello.world.config".GetPrimaryDomain() == "config.com");
    }

    [TestMethod]
    public void HexDarkenOrLighten_Success()
    {
        var value = "";
        var expected = "";
        var result = "";
        result = value.HexDarkenOrLighten();
        Assert.IsTrue(result == expected, "Result was " + result + " when expecting " + expected);

        value = "#12";
        expected = "";
        try
        {
            result = value.HexDarkenOrLighten();

            Assert.IsTrue(false, "Exception was not thrown");
        }
        catch
        {
            Assert.IsTrue(true);
        }

        value = "#000000";
        expected = "#FFFFFF";
        result = value.HexDarkenOrLighten(auto: true);
        Assert.IsTrue(result == expected, "Result was " + result + " when expecting " + expected);

        value = "#FFFFFF";
        expected = "#4F4F4F";
        result = value.HexDarkenOrLighten(auto: true);
        Assert.IsTrue(result == expected, "Result was " + result + " when expecting " + expected);

        value = "#414141";
        expected = "#EBEBEB";
        result = value.HexDarkenOrLighten(auto: true);
        Assert.IsTrue(result == expected, "Result was " + result + " when expecting " + expected);

        value = "#414141";
        expected = "#141414";
        result = value.HexDarkenOrLighten(auto: false);
        Assert.IsTrue(result == expected, "Result was " + result + " when expecting " + expected);

        value = "#414141";
        expected = "#555555";
        result = value.HexDarkenOrLighten(factor: -0.31, auto: false);
        Assert.IsTrue(result == expected, "Result was " + result + " when expecting " + expected);

        value = "#AAAAAA";
        expected = "#DFDFDF";
        result = value.HexDarkenOrLighten(factor: -0.31, auto: false);
        Assert.IsTrue(result == expected, "Result was " + result + " when expecting " + expected);

        value = "#DFDFDF";
        expected = "#252525";
        result = value.HexDarkenOrLighten(factor: -0.31, auto: false);
        Assert.IsTrue(result == expected, "Result was " + result + " when expecting " + expected);

        value = "#66FF00";
        expected = "#864F00";
        result = value.HexDarkenOrLighten(factor: -0.31, auto: false);
        Assert.IsTrue(result == expected, "Result was " + result + " when expecting " + expected);

        value = "#8f8f33";
        expected = "#CACA48";
        result = value.HexDarkenOrLighten(factor: -0.41, auto: false);
        Assert.IsTrue(result == expected, "Result was " + result + " when expecting " + expected);

        value = "#FFFFFF";
        expected = "#030303";
        result = value.HexDarkenOrLighten(factor: 0.99, auto: true);
        Assert.IsTrue(result == expected, "Result was " + result + " when expecting " + expected);

        value = "#8f8f33";
        expected = "#0101CD";
        result = value.HexDarkenOrLighten(factor: 0.99, auto: true);
        Assert.IsTrue(result == expected, "Result was " + result + " when expecting " + expected);

        value = "#030303";
        expected = "#FCFCFC";
        result = value.HexDarkenOrLighten(factor: 0.99, auto: true);
        Assert.IsTrue(result == expected, "Result was " + result + " when expecting " + expected);

        value = "#030303";
        expected = "#FFFFFF";
        result = value.HexDarkenOrLighten(factor: 0.01, auto: true);
        Assert.IsTrue(result == expected, "Result was " + result + " when expecting " + expected);

        value = "#FFFFFF";
        expected = "#4F4F4F";
        result = value.HexDarkenOrLighten(factor: 0.31, auto: true);
        Assert.IsTrue(result == expected, "Result was " + result + " when expecting " + expected);
    }

    [TestMethod]
    public void GetBytes_Success()
    {
        string text = null;
        byte[] bytes = null;

        bytes = text.GetBytes();

        Assert.IsTrue(bytes == null);

        text = "";
        bytes = text.GetBytes();
        Assert.IsTrue(bytes != null);
        Assert.IsTrue(bytes.Length == 0);

        text = "Hello world";
        bytes = text.GetBytes();

        Assert.IsTrue(text.Length == bytes.Length);
    }

    [TestMethod]
    public void UriEncode_Success()
    {
        var plain = "Hello world + ?";

        var coded = plain.UriEncode();

        Assert.IsTrue(coded == "Hello%20world%20%2B%20%3F");

        plain = null;
        coded = plain.UriEncode();
        Assert.IsTrue(coded == null);
    }

    [TestMethod]
    public void UriDecode_Success()
    {
        var coded = "Hello%20world%20%2B%20%3F";

        var plain = coded.UriDecode();

        Assert.IsTrue(plain == "Hello world + ?", plain);

        plain = null;
        coded = plain.UriDecode();
        Assert.IsTrue(coded == null);
    }

    [TestMethod]
    public void ToPascalCase_Success()
    {
        string text = null;
        string result = text.ToPascalCase();
        Assert.IsTrue(result == null);

        text = "";
        result = text.ToPascalCase();
        Assert.IsTrue(result == "");

        text = "1h";
        result = text.ToPascalCase();
        Assert.IsTrue(result == "1h");

        text = "a";
        result = text.ToPascalCase();
        Assert.IsTrue(result == "A");

        text = "HEllo world 1234this is nICE";
        result = text.ToPascalCase();
        Assert.IsTrue(result == "Hello World 1234this Is Nice", result);

        text = "HELLO WORLD@EMAIL.COM. This is it. this is just a sample? or is it?";
        result = text.ToPascalCase();
        Assert.IsTrue(result == "Hello World@email.com. This Is It. This Is Just A Sample? Or Is It?", result);

        text = "Hello-world-this WAS-NICE";
        result = text.ToPascalCase();
        Assert.IsTrue(result == "Hello-World-This Was-Nice");
    }

    [TestMethod]
    public void toCamelCase_Success()
    {
        string text = null;
        string result = text.toCamelCase();
        Assert.IsTrue(result == null);

        text = "";
        result = text.toCamelCase();
        Assert.IsTrue(result == "");

        text = "1h";
        result = text.toCamelCase();
        Assert.IsTrue(result == "1h");

        text = "A";
        result = text.toCamelCase();
        Assert.IsTrue(result == "a");

        text = "HEllo world 1234this is nICE";
        result = text.toCamelCase();
        Assert.IsTrue(result == "hello World 1234this Is Nice", result);

        text = "HELLO WORLD@EMAIL.COM. This is it. this is just a sample? or is it?";
        result = text.toCamelCase();
        Assert.IsTrue(result == "hello World@email.com. This Is It. This Is Just A Sample? Or Is It?", result);

        text = "Hello-world-this WAS-NICE";
        result = text.toCamelCase();
        Assert.IsTrue(result == "hello-World-This Was-Nice");

        text = "ApiControllerNameUnchanged";
        result = text.toCamelCase();
        Assert.IsTrue(result == "apiControllerNameUnchanged", result);
    }

    [TestMethod]
    public void ToUtf8BOM_Success()
    {
        var bomBytes = new byte[] {
            239,
            187,
            191
        };
        var bomChar = Encoding.UTF8.GetString(bomBytes);

        var test = "".ToUtf8BOM();
        var expected = "";

        Assert.IsTrue(test == expected, "Err1: " + test);

        test = "ÆØÅæøå?".ToUtf8BOM();
        expected = "ÆØÅæøå?";

        Assert.IsTrue(test == bomChar + expected, "Err2: " + test);

        test = "ÆØÅæøå?".ToUtf8BOM().ToUtf8BOM().ToUtf8BOM().ToUtf8BOM().ToUtf8BOM();
        expected = "ÆØÅæøå?";

        Assert.IsTrue(test == bomChar + expected, "Err3: " + test);

        test = "ÆØÅæøå?".ToUtf8BOM();
        expected = "ÆØÅæøå?".ToUtf8BOM();

        Assert.IsTrue(test == expected, "Err4: " + test);
    }

    [TestMethod]
    public void ToPhysicalPath_Success()
    {
        string text = null;
        string result = text.ToPhysicalPath();
        Assert.IsTrue(result == null);

        text = "";
        result = text.ToPhysicalPath();

        var root = "C:\\syslib\\systemlibrary-common-framework\\source\\systemlibrary.common.framework.net.tests\\bin\\Release\\net8.0\\";
        root = root.Replace("\\", "/");

        Assert.IsTrue(result == root, "1 " + result);

        text = "a";
        result = text.ToPhysicalPath();
        Assert.IsTrue(result == root + "a", "2 " + result);

        text = "a/b/c/d/e/12345/";
        result = text.ToPhysicalPath();
        Assert.IsTrue(result == root + "a/b/c/d/e/12345/", "3 " + result);

        text = "https://www.systemlibrary.com/hello/world/";
        result = text.ToPhysicalPath();
        Assert.IsTrue(result == root + "hello/world/", "4 " + result);

        text = "https://www.sub.sub.subdomain.com/hello/world/";
        result = text.ToPhysicalPath();
        Assert.IsTrue(result == root + "hello/world/", "5 " + result);

        text = "https://www.sub.sub.subdomain.com/hello1/world2/?hello=world&hello=/world/";
        result = text.ToPhysicalPath();
        Assert.IsTrue(result == root + "hello1/world2/", "6 " + result);

        text = "https://www.sub.sub.subdomain.com";
        result = text.ToPhysicalPath();
        Assert.IsTrue(result == root, "7 " + result);

        text = "/a/b/c";
        result = text.ToPhysicalPath();
        Assert.IsTrue(result == root + "a/b/c", "8 " + result);

        text = "/a/b/c/";
        result = text.ToPhysicalPath();
        Assert.IsTrue(result == root + "a/b/c/", "9 " + result);

        text = "\\a\\b\\";
        result = text.ToPhysicalPath();
        Assert.IsTrue(result == root + "a/b/", "10 " + result);

        text = "a\\b\\";
        result = text.ToPhysicalPath();
        Assert.IsTrue(result == root + "a/b/", "11 " + result);

        text = "a\\b";
        result = text.ToPhysicalPath();
        Assert.IsTrue(result == root + "a/b", "12 " + result);

        text = "C:/syslib/systemlibrary-common-framework/source/SystemLibrary.Common.Framework.Tests/a/";
        result = text.ToPhysicalPath();
        Assert.IsTrue(result == text, "13 " + result);

        text = "C:/syslib/systemlibrary-common-framework/source/SystemLibrary.Common.Framework.Tests/a";
        result = text.ToPhysicalPath();
        Assert.IsTrue(result == text, "14 " + result);

        text = "C:\\syslib\\systemlibrary-common-framework\\source\\SystemLibrary.Common.Framework.Tests\\a";
        result = text.ToPhysicalPath();
        Assert.IsTrue(result == "C:/syslib/systemlibrary-common-framework/source/SystemLibrary.Common.Framework.Tests/a", "15 " + result);

        text = "C:\\hello\\world";
        result = text.ToPhysicalPath();
        Assert.IsTrue(result == "C:/hello/world", "16 " + result);

        text = "C:\\hello\\world\\";
        result = text.ToPhysicalPath();
        Assert.IsTrue(result == "C:/hello/world/", "17 " + result);
    }

    [TestMethod]
    public void GetCompressedKey_IsOk()
    {
        var first = "";
        for (int i = 0; i < 100; i++)
        {
            string text = null;
            string result = text.GetCompressedKey();
            Assert.IsTrue(result == null);

            text = "";
            result = text.GetCompressedKey();
            Assert.IsTrue(result == "", result);

            text = "a";
            result = text.GetCompressedKey();
            Assert.IsTrue(result == text, "A " + result);

            text = "abcd";
            result = text.GetCompressedKey();
            Assert.IsTrue(result == text, result + ": " + result.Length + " vs " + text.Length + " == " + text);

            text = "abcde";
            result = text.GetCompressedKey();
            Assert.IsTrue(result.Length >= 3 && result.Length <= 5, result + ": " + result.Length + " vs " + text.Length + " == " + text);

            text = "abcdef";
            result = text.GetCompressedKey();
            Assert.IsTrue(result.Length >= 3 && result.Length <= 5, result + ": " + result.Length + " vs " + text.Length + " == " + text);

            text = "abcdefg";
            result = text.GetCompressedKey();
            Assert.IsTrue(result.Length >= 3 && result.Length <= 7, result + ": " + result.Length + " vs " + text.Length + " == " + text);
            if (first == "")
                first = result;
            Assert.IsTrue(first == result, "Next hashcompress within same appcontext generated a diff hash");

            text = "abcdefgh";
            result = text.GetCompressedKey();
            Assert.IsTrue(result.Length >= 4 && result.Length <= 7, result + ": " + result.Length + " vs " + text.Length + " == " + text);

            text = "abcdefghi";
            result = text.GetCompressedKey();
            Assert.IsTrue(result.Length >= 4 && result.Length <= 7, result + ": " + result.Length + " vs " + text.Length + " == " + text);

            text = "abcdefghij";
            result = text.GetCompressedKey();
            Assert.IsTrue(result.Length >= 5 && result.Length <= 8, result + ": " + result.Length + " vs " + text.Length + " == " + text);

            text = "abcdefghijk";
            result = text.GetCompressedKey();
            Assert.IsTrue(result.Length >= 5 && result.Length <= 8, result + ": " + result.Length + " vs " + text.Length + " == " + text);

            text = "abcdefghijkl";
            result = text.GetCompressedKey();
            Assert.IsTrue(result.Length >= 5 && result.Length <= 8, result + ": " + result.Length + " vs " + text.Length + " == " + text);

            text = "abcdefghijkllmn";
            result = text.GetCompressedKey();
            Assert.IsTrue(result.Length >= 5 && result.Length <= 8, result + ": " + result.Length + " vs " + text.Length + " == " + text);

            text = "abcdefghijabcdefghijabcdefghij";
            result = text.GetCompressedKey();
            Assert.IsTrue(result.Length >= 7 && result.Length <= 11, result + ": " + result.Length + " vs " + text.Length + " == " + text);

            text = "abcdefghijabcdefghijabcdefghij32";
            result = text.GetCompressedKey();
            Assert.IsTrue(result.Length >= 7 && result.Length <= 11, result + ": " + result.Length + " vs " + text.Length + " == " + text);

            text = "abcdefghijabcdefghijabcdefghij32abcdefghijabcdefghijabcdefghij64abcdefghijabcdefghijabcdefghij32abcdefghijabcdefghijabcdefghij64abcdefghijabcdefghijabcdefghij32abcdefghijabcdefghijabcdefghI192";
            result = text.GetCompressedKey();
            Assert.IsTrue(result.Length >= 7 && result.Length <= 11, result + ": " + result.Length + " vs " + text.Length + " == " + text);

            text = "abcdefghijabcdefghijabcdefghij32abcdefghijabcdefghijabcdefghij64abcdefghijabcdefghijabcdefghij32abcdefghijabcdefghijabcdefghij64abcdefghijabcdefghijabcdefghij32abcdefghijabcdefghijabcdefghij64abcdefghijabcdefghijabcdefghij32abcdefghijabcdefghijabcdefGHI256";
            result = text.GetCompressedKey();
            Assert.IsTrue(result.Length >= 7 && result.Length <= 11, result + ": " + result.Length + " vs " + text.Length + " == " + text);

            text = "abcdefghijabcdefghijabcdefghij32abcdefghijabcdefghijabcdefghij64abcdefghijabcdefghijabcdefghij32abcdefghijabcdefghijabcdefghij64abcdefghijabcdefghijabcdefghij32abcdefghijabcdefghijabcdefghij64abcdefghijabcdefghijabcdefghij32abcdefghijabcdefghijabcdefGHIJ257";
            result = text.GetCompressedKey();
            Assert.IsTrue(result.Length >= 7 && result.Length <= 11, result + ": " + result.Length + " vs " + text.Length + " == " + text);

            text = "abcdefghijabcdefghijabcdefghij32abcdefghijabcdefghijabcdefghij64abcdefghijabcdefghijabcdefghij32abcdefghijabcdefghijabcdefghij64abcdefghijabcdefghijabcdefghij32abcdefghijabcdefghijabcdefghij64abcdefghijabcdefghijabcdefghij32abcdefghijabcdefghijabcdefGHI256abcdefghijabcdefghijabcdefghij32abcdefghijabcdefghijabcdefghij64abcdefghijabcdefghijabcdefghij32abcdefghijabcdefghijabcdefghij64abcdefghijabcdefghijabcdefghij32abcdefghijabcdefghijabcdefghij64abcdefghijabcdefghijabcdefghij32abcdefghijabcdefghijabcdefGHI256abcdefghijabcdefghijabcdefghij32abcdefghijabcdefghijabcdefghij64abcdefghijabcdefghijabcdefghij32abcdefghijabcdefghijabcdefghij64abcdefghijabcdefghijabcdefghij32abcdefghijabcdefghijabcdefghij64abcdefghijabcdefghijabcdefghij32abcdefghijabcdefghijabcdefGHI256abcdefghijabcdefghijabcdefghij32abcdefghijabcdefghijabcdefghij64abcdefghijabcdefghijabcdefghij32abcdefghijabcdefghijabcdefghij64abcdefghijabcdefghijabcdefghij32abcdefghijabcdefghijabcdefghij64abcdefghijabcdefghijabcdefghij32abcdefghijabcdefghijabcdefGHI256abcdefghijabcdefghijabcdefghij32abcdefghijabcdefghijabcdefghij64abcdefghijabcdefghijabcdefghij32abcdefghijabcdefghijabcdefghij64abcdefghijabcdefghijabcdefghij32abcdefghijabcdefghijabcdefghij64abcdefghijabcdefghijabcdefghij32abcdefghijabcdefghijabcdefGHI256abcdefghijabcdefghijabcdefghij32abcdefghijabcdefghijabcdefghij64abcdefghijabcdefghijabcdefghij32abcdefghijabcdefghijabcdefghij64abcdefghijabcdefghijabcdefghij32abcdefghijabcdefghijabcdefghij64abcdefghijabcdefghijabcdefghij32abcdefghijabcdefghijabcdefGHI256abcdefghijabcdefghijabcdefghij32abcdefghijabcdefghijabcdefghij64abcdefghijabcdefghijabcdefghij32abcdefghijabcdefghijabcdefghij64abcdefghijabcdefghijabcdefghij32abcdefghijabcdefghijabcdefghij64abcdefghijabcdefghijabcdefghij32abcdefghijabcdefghijabcdefGHI256abcdefghijabcdefghijabcdefghij32abcdefghijabcdefghijabcdefghij64abcdefghijabcdefghijabcdefghij32abcdefghijabcdefghijabcdefghij64abcdefghijabcdefghijabcdefghij32abcdefghijabcdefghijabcdefghij64abcdefghijabcdefghijabcdefghij32abcdefghijabcdefghijabcdefGHI256abcdefghijabcdefghijabcdefghij32abcdefghijabcdefghijabcdefghij64abcdefghijabcdefghijabcdefghij32abcdefghijabcdefghijabcdefghij64abcdefghijabcdefghijabcdefghij32abcdefghijabcdefghijabcdefghij64abcdefghijabcdefghijabcdefghij32abcdefghijabcdefghijabcdefGHI256abcdefghijabcdefghijabcdefghij32abcdefghijabcdefghijabcdefghij64abcdefghijabcdefghijabcdefghij32abcdefghijabcdefghijabcdefghij64abcdefghijabcdefghijabcdefghij32abcdefghijabcdefghijabcdefghij64abcdefghijabcdefghijabcdefghij32abcdefghijabcdefghijabcdefGHI256abcdefghijabcdefghijabcdefghij32abcdefghijabcdefghijabcdefghij64abcdefghijabcdefghijabcdefghij32abcdefghijabcdefghijabcdefghij64abcdefghijabcdefghijabcdefghij32abcdefghijabcdefghijabcdefghij64abcdefghijabcdefghijabcdefghij32abcdefghijabcdefghijabcdefGHI256abcdefghijabcdefghijabcdefghij32abcdefghijabcdefghijabcdefghij64abcdefghijabcdefghijabcdefghij32abcdefghijabcdefghijabcdefghij64abcdefghijabcdefghijabcdefghij32abcdefghijabcdefghijabcdefghij64abcdefghijabcdefghijabcdefghij32abcdefghijabcdefghijabcdefGHI3096";
            result = text.GetCompressedKey();
            Assert.IsTrue(result == "70323g3073", result + ": " + result.Length + " vs " + text.Length + " == " + text);

            // 12800 chars
            text = "zzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzZZZZzzzzZZZZzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz";
            result = text.GetCompressedKey();
            Assert.IsTrue(result == "89914z12800", result + ": " + result.Length + " vs " + text.Length + " == " + text);
        }
    }

    [TestMethod]
    public void GetCompressedKey_Generates_Same_key_In_Async_IsOK()
    {
        var result = Async.Tasks(
            () => OnAsyncRun("Hello world"),
            () => OnAsyncRun("Hello world"),
            () => OnAsyncRun("Hello world"),
            () => OnAsyncRun("Hello world"),
            () => OnAsyncRun("Hello world"),
            () => OnAsyncRun("Hello world")
        );

        Assert.IsTrue(result.Count == 6, "Async.Run did not execute all 6,: " + result.Count);

        var first = result[0];

        Assert.IsTrue(first.Length > 3, "Too short hash " + first);

        foreach (var equals in result)
            Assert.IsTrue(first == equals, "First did not match: " + equals);
    }

    static string OnAsyncRun(string input)
    {
        return input.GetCompressedKey();
    }
}