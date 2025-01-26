using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.Extensions;
using SystemLibrary.Common.Framework.Tests;

namespace SystemLibrary.Common.Framework;

[TestClass]
public class ArrayExtensionsTests : BaseTest
{
    [TestMethod]
    public void Add_Array_To_Empty_Array()
    {
        var names = new string[] { };

        var res = names.Add(new string[] { "World", "world", "w0rld" });

        Assert.IsTrue(res.Length == 3);
    }

    [TestMethod]
    public void Add_Empty_Array_To_Empty_Array()
    {
        var names = new string[] { };

        var res = names.Add(new string[] { });

        Assert.IsTrue(res.Length == 0);
    }

    [TestMethod]
    public void Add_Empty_Array_To_Null_Array()
    {
        string[] names = null;

        var res = names.Add(new string[] { });

        Assert.IsTrue(res.Length == 0);
    }

    [TestMethod]
    public void Add_Null_Array_To_Null_Array()
    {
        string[] names = null;

        string[] add = null;

        var res = names.Add(add);

        Assert.IsTrue(res == null);
    }

    [TestMethod]
    public void Add_Array_To_Array()
    {
        var names = new string[] { "Hello", "hello", "hell0" };

        var res = names.Add(new string[] { "World", "world", "w0rld" });

        Assert.IsTrue(res.Length == 6);
    }

    [TestMethod]
    public void Add_Int_Array_To_Int_Array()
    {
        var names = new int[] { 1, 2, 3, 4 };

        var res = names.Add(new int[] { 1, 2, 3, 4, 5 });

        Assert.IsTrue(res.Length == 9);
    }

    static bool IntPredicate(int number)
    {
        return number > 2;
    }

    [TestMethod]
    public void Add_Int_Array_To_Int_Array_With_Predicate()
    {
        var numbers = new int[] { 1, 2, 3, 4 };

        var res = numbers.Add(IntPredicate, new int[] { 1, 2, 3, 4, 5 });

        Assert.IsTrue(res.Length == 5);
    }

    [TestMethod]
    public void Add_Multiple_Int_Arrays_To_Int_Array_With_Predicate()
    {
        var numbers = new int[] { 1, 2, 3, 4 };
        var add1 = new int[] { 1, 2, 3, 4, 5 };
        var add2 = new int[] { 5, 6, 7, 8 };
        var add3 = new int[] { 0, 1, -1, -2, 9 };

        var res = numbers.Add(IntPredicate, add1, add2, add3);

        Assert.IsTrue(res.Length == 10);
    }
}
