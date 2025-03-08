namespace SystemLibrary.Common.Framework.Extensions;

/// <summary>
/// Extension methods for IList of strings
/// </summary>
public static class IListStringExtensions
{
    /// <summary>
    /// Convert an IList of string to a new List of Enum
    /// </summary>
    /// <remarks>
    /// Does not Throw exception
    /// Tries to convert data through EnumValue and EnumText if match is found
    /// </remarks>
    /// <example>
    /// Example:
    /// <code>
    /// enum Cars
    /// {
    ///   Car1,
    ///   [EnumValue("Second car value")]
    ///   Car2,
    ///   [EnumText("Third Car Display Text")]
    ///   Car3,
    ///   [EnumText("Fourth Car Display Text")]
    ///   Car4
    /// }
    /// 
    /// var list = new List&lt;string&gt; { null, "", "SECOND CAR VALUE", "car4", "fourth car display text"};
    /// 
    /// var enums = list.ToEnumList&lt;Cars&gt;();
    /// 
    /// // enums[0] == Car1, null becomes the default value
    /// // enums[1] == Car1, empty string becomes the default value
    /// // enums[2] == Car2, case insensitive match
    /// // enums[3] == Car4, case insensitive match
    /// // enums[4] == Car4, case insensitive match
    /// </code>
    /// </example>
    /// <returns>A new list with 0 or more Enums</returns>
    public static List<TEnum> ToEnumList<TEnum>(this IList<string> collection) where TEnum : struct, IComparable, IFormattable, IConvertible
    {
        if (collection.IsNot())
            return new List<TEnum>();

        var items = new List<TEnum>();

        foreach (var item in collection)
        {
            items.Add(item.ToEnum<TEnum>());
        }
        return items;
    }
}
