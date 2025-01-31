using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework;

[TestClass]
public class TypeExtensionsTests
{
    [TestMethod]
    public void Inherits_Success()
    {
        var stringType = typeof(string);
        var objectType = typeof(object);
        var listType = typeof(List<int>);
        var enumerableType = typeof(IEnumerable<int>);

        var res = stringType.Inherits(objectType);
        Assert.IsTrue(res);

        res = objectType.Inherits(objectType);
        Assert.IsTrue(!res);

        res = objectType.Inherits(stringType);
        Assert.IsTrue(!res);

        res = listType.Inherits(objectType);
        Assert.IsTrue(res);

        res = listType.Inherits(enumerableType);
        Assert.IsTrue(res);
    }

    [TestMethod]
    public void IsClassType_Success()
    {
        Assert.IsTrue(!SystemType.Int16Type.IsClassType(), "Int16");
        Assert.IsTrue(!SystemType.IntType.IsClassType(), "IntType");
        Assert.IsTrue(!SystemType.Int64Type.IsClassType(), "Int64Type");
        Assert.IsTrue(!SystemType.StringType.IsClassType(), "StringType");
        Assert.IsTrue(!SystemType.StringBuilderType.IsClassType(), "StringBuilderType");
        Assert.IsTrue(!SystemType.UInt64Type.IsClassType(), "UInt64Type");
        Assert.IsTrue(!SystemType.UIntType.IsClassType(), "UIntType");
        Assert.IsTrue(!SystemType.IListType.IsClassType(), "IListType");
        Assert.IsTrue(!SystemType.DictionaryType.IsClassType(), "DictionaryType");
        Assert.IsTrue(!SystemType.BoolType.IsClassType(), "BoolType");
        Assert.IsTrue(!SystemType.NullableType.IsClassType(), "NullableType");
        Assert.IsTrue(!SystemType.KeyValueType.IsClassType(), "KeyValueType");
        Assert.IsTrue(typeof(TypeExtensionsTests).IsClassType(), "TypeExtensionsTests");
        Assert.IsTrue(!typeof(Tuple).IsClassType(), "Tuple");
        Assert.IsTrue(!typeof(Tuple<int,string>).IsClassType(), "Tuple<>");
    }

    [TestMethod]
    public void IsKeyValuePairType()
    {
        var type = typeof(string);

        Assert.IsTrue(!type.IsKeyValuePair());

        type = typeof(object);
        Assert.IsTrue(!type.IsKeyValuePair());

        type = typeof(List<int>);
        Assert.IsTrue(!type.IsKeyValuePair());
        type = typeof(IEnumerable<int>);
        Assert.IsTrue(!(type.IsKeyValuePair()));

        type = typeof(IEnumerable<object>);
        Assert.IsTrue(!(type.IsKeyValuePair()));

        type = typeof(KeyValuePair<int, int>);
        Assert.IsTrue(type.IsKeyValuePair());

        type = typeof(KeyValuePair<int, object>);
        Assert.IsTrue(type.IsKeyValuePair());

        type = typeof(KeyValuePair<string, int>);
        Assert.IsTrue(type.IsKeyValuePair());

        type = typeof(KeyValuePair<int, string>);
        Assert.IsTrue(type.IsKeyValuePair());
    }

    [TestMethod]
    public void Type_Get_Type_Argument()
    {
        var type = typeof(IList<string>);
        var expected = typeof(string);
        var res = type.GetTypeArgument();

        Assert.IsTrue(res == expected);

        type = typeof(string);
        expected = null;
        res = type.GetTypeArgument();
        Assert.IsTrue(res == expected);

        type = null;
        expected = null;
        res = type.GetTypeArgument();
        Assert.IsTrue(res == expected);

        type = typeof(int);
        expected = null;
        res = type.GetTypeArgument();
        Assert.IsTrue(res == expected, "int");

        type = typeof(List<int>);
        expected = typeof(int);
        res = type.GetTypeArgument();
        Assert.IsTrue(res == expected, "List int");

        type = typeof(int[]);
        expected = typeof(int);
        res = type.GetTypeArgument();
        Assert.IsTrue(res == expected, "Array int");

        type = typeof(Dictionary<string, object>);
        expected = typeof(string);
        res = type.GetTypeArgument();
        Assert.IsTrue(res == expected, "Dictionary keyvaluepair: " + res.Name);

        type = typeof(Dictionary<int, List<int>>);
        expected = typeof(int);
        res = type.GetTypeArgument();
        Assert.IsTrue(res == expected, "Dictionary keyvaluepair list of int");

        type = typeof(string[]);
        expected = typeof(string);
        res = type.GetTypeArgument();
        Assert.IsTrue(res == expected, "Array str");

        type = typeof(Tuple<DateTime, string, int, bool>[]);
        expected = typeof(DateTime);
        res = type.GetTypeArgument();
        Assert.IsTrue(res == expected, "Tuple date");

        res = type.GetTypeArgument(1);
        expected = typeof(string);
        Assert.IsTrue(res == expected, "Tuple str");

        res = type.GetTypeArgument(2);
        expected = typeof(int);
        Assert.IsTrue(res == expected, "Tuple int");

        res = type.GetTypeArgument(3);
        expected = typeof(bool);
        Assert.IsTrue(res == expected, "Tuple b");

        type = typeof(bool?);
        expected = typeof(bool);
        res = type.GetTypeArgument();
        Assert.IsTrue(res == expected, "nullable bool");

        type = typeof(Dictionary<int, List<int>>);
        expected = typeof(List<int>);
        res = type.GetTypeArgument(1);
        Assert.IsTrue(res == expected, "Dictionary keyvaluepair list of int is wrong");

        type = typeof(Dictionary<int, List<int>>);
        expected = typeof(List<int>);

        var res2 = type.GetTypeArguments();

        Assert.IsTrue(res2.Length == 2, "GetTypeArguments wrong count " + res2.Length);
        Assert.IsTrue(res2[0] == typeof(int), "GetTypeArguments wrong index 0");
        Assert.IsTrue(res2[1] == typeof(List<int>), "GetTypeArguments wrong index 1");
    }

    [TestMethod]
    public void IsList_Or_Array_Success()
    {
        var list = new List<int>();

        var res = list.GetType().IsListOrArray();

        Assert.IsTrue(res, "List");

        var a = new List<int>();
        var b = (object)a;
        var ilist = (IList<int>)b;

        res = ilist.GetType().IsListOrArray();

        Assert.IsTrue(res, "IList");

        var arr = new int[1];

        res = arr.GetType().IsListOrArray();
        Assert.IsTrue(res, "Array");
    }

    [TestMethod]
    public void GetTypeName_Success()
    {
        var type = typeof(string);
        var name = type.GetTypeName();
        Assert.IsTrue("String" == name, name);

        type = typeof(int);
        name = type.GetTypeName();
        Assert.IsTrue("Int32" == name, name);

        type = typeof(List<>);
        name = type.GetTypeName();
        Assert.IsTrue("<>" == name, name);

        type = typeof(List<int>);
        name = type.GetTypeName();
        Assert.IsTrue("Int32" == name, name);

        type = typeof(List<string>);
        name = type.GetTypeName();
        Assert.IsTrue("String" == name, name);

        type = typeof(IList<DateTimeOffset>);
        name = type.GetTypeName();
        Assert.IsTrue("DateTimeOffset" == name, name);

        type = typeof(IDictionary<,>);
        name = type.GetTypeName();
        Assert.IsTrue("<,>" == name, name);

        type = typeof(IDictionary<object, int>);
        name = type.GetTypeName();
        Assert.IsTrue("Object, Int32" == name, name);
    }

    [TestMethod]
    public void IsDictionary()
    {
        var type = typeof(string);
        var res = type.IsDictionary();
        Assert.IsTrue(res == false);

        type = typeof(int[]);
        res = type.IsDictionary();
        Assert.IsTrue(res == false);

        type = typeof(List<string>);
        res = type.IsDictionary();
        Assert.IsTrue(res == false);

        type = typeof(IList<string>);
        res = type.IsDictionary();
        Assert.IsTrue(res == false);

        type = typeof(IDictionary<string, int>);
        res = type.IsDictionary();
        Assert.IsTrue(res == true);

        type = typeof(IDictionary<object, object>);
        res = type.IsDictionary();
        Assert.IsTrue(res == true);

        type = typeof(Dictionary<int, bool>);
        res = type.IsDictionary();
        Assert.IsTrue(res == true);
    }

    [TestMethod]
    public void Test_Is_Internal()
    {
        var type = typeof(string);
        var res = type.IsInternal();
        Assert.IsTrue(res == false, "Its true string");

        type = typeof(NonExistingConfig);
        res = type.IsInternal();
        Assert.IsTrue(res == true, "Internal is not internal");
    }
}
