using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.Extensions;
using SystemLibrary.Common.Framework.Tests;

namespace SystemLibrary.Common.Framework;

[TestClass]
public class StringBuilderExtensionsTests : BaseTest
{
    [TestMethod]
    public void TrimEnd_Success()
    {
        StringBuilder sb = null;
        bool result = false;
        bool expected = false;

        result = sb.TrimEnd("abc");
        Assert.IsTrue(result == expected, "abc 1 errored");

        sb = new StringBuilder();
        result = sb.TrimEnd("abc");
        Assert.IsTrue(result == expected, "abc 2 errored");

        sb = new StringBuilder("");
        result = sb.TrimEnd("abc");
        Assert.IsTrue(result == expected, "abc 3 errored");
        Assert.IsTrue(sb.ToString() == "", "abc 3 not equal empty");

        sb = new StringBuilder("ab");
        result = sb.TrimEnd("abc");
        Assert.IsTrue(result == expected, "abc 4 errored");
        Assert.IsTrue(sb.ToString() == "ab", "abc 4 not equal ab");

        sb = new StringBuilder("abc");
        result = sb.TrimEnd("abc");
        expected = true;
        Assert.IsTrue(result == expected, "abc 5 errored");
        Assert.IsTrue(sb.ToString() == "", "abc was not removed");

        sb = new StringBuilder("Hello World !234567890-.-,_:_");
        result = sb.TrimEnd("Hello World !234567890-.-,_:_");
        expected = true;
        Assert.IsTrue(result == expected, "Hello world errored");
        Assert.IsTrue(sb.ToString() == "", "Hello world was not removed");

        sb = new StringBuilder("ABC");
        result = sb.TrimEnd("abC");
        expected = false;
        Assert.IsTrue(result == expected, "abC 1 errored");
        Assert.IsTrue(sb.ToString() == "ABC", "abC was not removed");

        sb = new StringBuilder("ABC");
        result = sb.TrimEnd("ABC ");
        expected = false;
        Assert.IsTrue(result == expected, "ABC 2 errored");
        Assert.IsTrue(sb.ToString() == "ABC", "ABC was not removed");

        sb = new StringBuilder("1234567890?ÆØÅÆØÅ");
        result = sb.TrimEnd("ÆØÅ");
        expected = true;
        Assert.IsTrue(result == expected, "ÆØÅ 3 errored");
        Assert.IsTrue(sb.ToString() == "1234567890?ÆØÅ", "Only one ÆØÅ was removed");

        sb = new StringBuilder("ceo@systemlibrary.com");
        result = sb.TrimEnd("ceo@SYstemlibRary.COM");
        expected = false;
        Assert.IsTrue(result == expected, "Email errored");
        Assert.IsTrue(sb.ToString() == "ceo@systemlibrary.com", "Email was not removed");

        sb = new StringBuilder("ceo@SYstemlibRary.COM");
        result = sb.TrimEnd("ceo@SYstemlibRary.COM");
        expected = true;
        Assert.IsTrue(result == expected, "Email errored");
        Assert.IsTrue(sb.ToString() == "", "Email was not removed");

        sb = new StringBuilder("hello world this is the end this is the end this is the end");
        result = sb.TrimEnd("hello", "world", "or", "this is the end");
        sb.TrimEnd(" ");
        expected = true;
        Assert.IsTrue(result == expected, "Hello world 2 multiple errored");
        Assert.IsTrue(sb.ToString() == "hello world this is the end this is the end", "Hello world 2 multiple was not removed");


        sb = new StringBuilder("Hello world \t\n\n\n");
        result = sb.TrimEnd("\n");
        Assert.IsTrue(result, "Hello world tab and new lines did not trim new line");
        Assert.IsTrue(sb.ToString() == "Hello world \t\n\n");
    }

    [TestMethod]
    public void EndsWith_Success()
    {
        StringBuilder sb = null;
        bool result = false;
        bool expected = false;

        result = sb.EndsWith("abc");
        Assert.IsTrue(result == expected, "abc 1 errored");

        sb = new StringBuilder();
        result = sb.EndsWith("abc");
        Assert.IsTrue(result == expected, "abc 2 errored");

        sb = new StringBuilder("");
        result = sb.EndsWith("abc");
        Assert.IsTrue(result == expected, "abc 3 errored");

        sb = new StringBuilder("ab");
        result = sb.EndsWith("abc");
        Assert.IsTrue(result == expected, "abc 4 errored");

        sb = new StringBuilder("abc");
        result = sb.EndsWith("abc");
        expected = true;
        Assert.IsTrue(result == expected, "abc 5 errored");

        sb = new StringBuilder("ABCD");
        result = sb.EndsWith("abc");
        expected = false;
        Assert.IsTrue(result == expected, "abc 6 errored");

        sb = new StringBuilder("abcdef 12354-.-1-51.64+06913+0|13125?!?!:__;:");
        result = sb.EndsWith("abc");
        expected = false;
        Assert.IsTrue(result == expected, "abc 7 errored");

        sb = new StringBuilder("abcdef 12354-.-1-51.64+06913+0|13125?!?!:__;:abc");
        result = sb.EndsWith("abc");
        expected = true;
        Assert.IsTrue(result == expected, "abc 8 errored");

        sb = new StringBuilder("abcdef 12354-.-1-51.64+06913+0|13125?!?!:__;:abc");
        result = sb.EndsWith("ABC");
        expected = false;
        Assert.IsTrue(result == expected, "abc 9 errored");

        sb = new StringBuilder("abc");
        result = sb.EndsWith("ABC", true);
        expected = true;
        Assert.IsTrue(result == expected, "abc 10 errored");


        sb = new StringBuilder("Hello World æøå");
        result = sb.EndsWith("Å", true);
        expected = true;
        Assert.IsTrue(result == expected, "ÆØÅ 1 errored");

        sb = new StringBuilder("Hello World æøå");
        result = sb.EndsWith("æøå", true);
        expected = true;
        Assert.IsTrue(result == expected, "ÆØÅ 2 errored");

        sb = new StringBuilder("Hello World ÆØÅ");
        result = sb.EndsWith("æøå", true);
        expected = true;
        Assert.IsTrue(result == expected, "ÆØÅ 3 errored");

        sb = new StringBuilder("Hello World Æ Ø Å");
        result = sb.EndsWith(" Æ Ø å", true);
        expected = true;
        Assert.IsTrue(result == expected, "ÆØÅ 4 errored");

        sb = new StringBuilder("Hello World æøå");
        result = sb.EndsWith("Å", false);
        expected = false;
        Assert.IsTrue(result == expected, "ÆØÅ 5 errored");

        sb = new StringBuilder("Hello World æøå");
        result = sb.EndsWith("æøå");
        expected = true;
        Assert.IsTrue(result == expected, "ÆØÅ 6 errored");

        sb = new StringBuilder("Hello World ÆØÅ");
        result = sb.EndsWith("æøå");
        expected = false;
        Assert.IsTrue(result == expected, "ÆØÅ 7 errored");

        sb = new StringBuilder("Hello World Æ Ø Å");
        result = sb.EndsWith(" Æ Ø å");
        expected = false;
        Assert.IsTrue(result == expected, "ÆØÅ 8 errored");
    }

    [TestMethod]
    public void IndexOf_Success()
    {
        StringBuilder sb = null;
        int result = 0;
        int expected = -1;

        result = sb.IndexOf("abc");
        Assert.IsTrue(result == expected, "abc 1 errored");

        sb = new StringBuilder();
        result = sb.IndexOf("abc");
        Assert.IsTrue(result == expected, "abc 2 errored");

        sb = new StringBuilder("");
        result = sb.IndexOf("abc");
        Assert.IsTrue(result == expected, "abc 3 errored");

        sb = new StringBuilder("ab");
        result = sb.IndexOf("abc");
        Assert.IsTrue(result == expected, "abc 4 errored");

        sb = new StringBuilder("abc");
        result = sb.IndexOf("abc");
        expected = 0;
        Assert.IsTrue(result == expected, "abc 5 errored " + result);

        sb = new StringBuilder(" abc");
        result = sb.IndexOf("abc");
        expected = 1;
        Assert.IsTrue(result == expected, "abc 6 errored " + result);

        sb = new StringBuilder("ABC abc");
        result = sb.IndexOf("abc");
        expected = 4;
        Assert.IsTrue(result == expected, "abc 7 errored " + result);

        sb = new StringBuilder("\t\t\t\t\n\n\nn\nHELLO WORLDabcDEF!");
        result = sb.IndexOf("abc");
        expected = 20;
        Assert.IsTrue(result == expected, "abc 8 errored " + result);

        sb = new StringBuilder("\t\t\t\t\n\n\nn\nHELLO WORLDabc");
        result = sb.IndexOf("abc");
        expected = 20;
        Assert.IsTrue(result == expected, "abc 9 errored " + result);

        sb = new StringBuilder("abc\t\t\t\t\n\n\nn\nHELLO WORLDabc");
        result = sb.IndexOf("abc");
        expected = 0;
        Assert.IsTrue(result == expected, "abc 10 errored " + result);

        sb = new StringBuilder("\t\t\t\t\n\n\nn\nHELLO WORLDabcDEF!");
        result = sb.IndexOf("ABC", true);
        expected = 20;
        Assert.IsTrue(result == expected, "abc 11 errored " + result);

        sb = new StringBuilder("\t\t\t\t\n\n\nn\nHELLO WORLDDEF!      ABC");
        result = sb.IndexOf("abc", true);
        expected = 30;
        Assert.IsTrue(result == expected, "abc 12 errored " + result);

        sb.Insert(result, "HELLO");
        result = sb.IndexOf("helloAbC", true);
        Assert.IsTrue(result == expected, "abc 13 errored " + result);
    }

    [TestMethod]
    public void HtmlDecodeQuotes_Success()
    {
        var sb = new StringBuilder("& < > &quot; &apos; and NOR: ÆØÅæøå <p></p>");

        sb.HtmlDecodeQuotes();

        var res = sb.ToString();

        Assert.IsTrue(res.Contains("'"));
        Assert.IsTrue(res.Contains("& "));
        Assert.IsTrue(res.Contains("\""));
        Assert.IsTrue(res.Contains("<"));
        Assert.IsTrue(res.Contains(">"));
        Assert.IsTrue(res.Contains("Ø"));
        Assert.IsTrue(res.Contains("ø"));
    }

    [TestMethod]
    public void HtmlEncodeQuotes_Success()
    {
        var sb = new StringBuilder("& < > \" ' and NOR: ÆØÅæøå <p></p>");

        sb.HtmlEncodeQuotes();

        var res = sb.ToString();

        Assert.IsTrue(res.Contains("&quot;"));
        Assert.IsTrue(res.Contains("& "));
        Assert.IsTrue(res.Contains("&apos;"));
        Assert.IsTrue(res.Contains("<"));
        Assert.IsTrue(res.Contains(">"));
        Assert.IsTrue(res.Contains("Ø"));
        Assert.IsTrue(res.Contains("ø"));
    }

    [TestMethod]
    public void MaxLength_Success()
    {
        var sb = new StringBuilder("hello world");
        sb.MaxLength(1);
        Assert.IsTrue(sb.ToString() == "h", "Max length 1 failed " + sb);

        sb.Append("ello");
        sb.MaxLength(3);
        Assert.IsTrue(sb.ToString() == "hel", "Max length 3 failed " + sb);
    }
}
