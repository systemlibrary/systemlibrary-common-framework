using System.Collections;
using System.Text;

using SystemLibrary.Common.Framework.Attributes;

namespace SystemLibrary.Common.Framework;

/// <summary>
/// Static class containing common "typeof" calls for performance reasons
/// </summary>
/// <remarks>
/// typeof is 'slow' so whenever you remember, you should use any of these types
/// </remarks>
public static class SystemType
{
    public static Type StringType = typeof(string);
    public static Type StringBuilderType = typeof(StringBuilder);
    public static Type Int16Type = typeof(short);
    public static Type IntType = typeof(int);
    public static Type Int64Type = typeof(long);
    public static Type UIntType = typeof(uint);
    public static Type UInt64Type = typeof(ulong);
    public static Type DateTimeType = typeof(DateTime);
    public static Type DateTimeOffsetType = typeof(DateTimeOffset);
    public static Type TimeSpanType = typeof(TimeSpan);
    public static Type BoolType = typeof(bool);
    public static Type GuidType = typeof(Guid);
    public static Type CharType = typeof(char);
    public static Type DoubleType = typeof(double);
    public static Type UriType = typeof(Uri);

    public static Type ExceptionType = typeof(Exception);

    public static Type ListType = typeof(List<>);
    public static Type IListType = typeof(IList<>);
    public static Type DictionaryType = typeof(Dictionary<,>);
    public static Type IDictionaryType = typeof(IDictionary<,>);

    public static Type ObjectType = typeof(object);

    public static Type EnumValueAttributeType = typeof(EnumValueAttribute);
    public static Type EnumTextAttributeType = typeof(EnumTextAttribute);

    public static Type Int16TypeNullable = typeof(short?);
    public static Type IntTypeNullable = typeof(int?);
    public static Type Int64TypeNullable = typeof(long?);
    public static Type BoolTypeNullable = typeof(bool?);
    public static Type DateTimeTypeNullable = typeof(DateTime?);
    public static Type TimeSpanTypeNullable = typeof(TimeSpan?);
    public static Type DateTimeOffsetTypeNullable = typeof(DateTimeOffset?);
    public static Type DoubleTypeNullable = typeof(double?);
    public static Type ICollectionType = typeof(ICollection);
    public static Type ICollectionGenericType = typeof(ICollection<>);
    public static Type KeyValueType = typeof(KeyValuePair<,>);
    public static Type DelegateType = typeof(Delegate);
    public static Type NullableType = typeof(Nullable<>);
    public static Type TupleType = typeof(Tuple);
}
