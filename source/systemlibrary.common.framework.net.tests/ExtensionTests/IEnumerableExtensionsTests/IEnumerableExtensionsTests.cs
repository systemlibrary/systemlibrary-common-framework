using System.Collections.ObjectModel;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.Extensions;
using SystemLibrary.Common.Framework.Tests;

namespace SystemLibrary.Common.Framework;

[TestClass]
public class IEnumerableExtensionsTests : BaseTest
{
    [TestMethod]
    public void List_Is_Not()
    {
        List<string> list = null;
        Assert.IsTrue(list.IsNot(), "List is not null/empty");

        list = new List<string>();
        Assert.IsTrue(list.IsNot(), "List is not null/empty");

        list.Add("");
        Assert.IsFalse(list.IsNot(), "List is null/empty when it has 1 item");
    }

    [TestMethod]
    public void Array_Is_Not()
    {
        int[] array = null;
        Assert.IsTrue(array.IsNot(), "Array is not null/empty");

        array = new int[0];
        Assert.IsTrue(array.IsNot(), "Array is not null/empty");

        array = new int[1] { 0 };
        Assert.IsFalse(array.IsNot(), "Array is null/empty when it has 1 item");
    }

    [TestMethod]
    public void Dictionary_Is_Not()
    {
        Dictionary<string, object> dictionary = null;
        Assert.IsTrue(dictionary.IsNot(), "Dictionary is not null/empty");

        dictionary = new Dictionary<string, object>();
        Assert.IsTrue(dictionary.IsNot(), "Dictionary is not null/empty");

        dictionary.Add("Hello", null);
        Assert.IsFalse(dictionary.IsNot(), "Dictionary is null/empty when it has 1 item");
    }

    [TestMethod]
    public void Collection_Is_Not()
    {
        Collection<string> collection = null;
        Assert.IsTrue(collection.IsNot(), "Collection is not null/empty");

        collection = new Collection<string>();
        Assert.IsTrue(collection.IsNot(), "Collection is not null/empty");

        collection.Add(null);
        Assert.IsTrue(collection.IsNot(), "Collection is null/empty when it has 1 item");

        collection.Add(null);
        Assert.IsFalse(collection.IsNot(), "Collection is null/empty when it has 1 item");
    }

    [TestMethod]
    public void Enumerable_Is_Not()
    {
        IEnumerable<int> GetValues(int count)
        {
            for (int i = 0; i < count; i++)
                yield return i;
        }

        IEnumerable<int> enumerable = null;
        Assert.IsTrue(enumerable.IsNot(), "Enumerable is not null/empty");

        enumerable = GetValues(0);
        Assert.IsTrue(enumerable.IsNot(), "Enumerable is not null/empty2");

        enumerable = GetValues(1);
        Assert.IsFalse(enumerable.IsNot(), "Enumerable is null/empty when it has 1 item");
    }

    [TestMethod]
    public void Enumerable_Has()
    {
        var texts = new string[] { "Hello", "World" };
        Assert.IsFalse(texts.Has(""));
        Assert.IsTrue(texts.Has("Hello"));
        Assert.IsFalse(texts.Has("hello"));

        Enum[] enums = new Enum[] { BackgroundColor.Red, BackgroundColor.Blue };

        Assert.IsTrue(enums.Has(BackgroundColor.Red));
        Assert.IsTrue(enums.Has(BackgroundColor.Blue));
        Assert.IsFalse(enums.Has(BackgroundColor.Green));

        var employees = Employee.CreateList();
        var employee = employees[0];

        var employee2 = new Employee();

        Assert.IsTrue(employees.Has(employee), "Missing employee");
        Assert.IsFalse(employees.Has(employee2), "Has employee2");
    }
}
