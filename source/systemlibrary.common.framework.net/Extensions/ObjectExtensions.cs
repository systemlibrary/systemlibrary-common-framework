namespace SystemLibrary.Common.Framework.Extensions;

/// <summary>
/// This class contains extension methods on Object
/// </summary>
public static class ObjectExtensions
{
    /// <summary>
    /// Join multiple arrays of same 'Enum' to an Array of that Enum Type
    /// </summary>
    /// <example>
    /// <code class="language-csharp hljs">
    /// public enum Colors
    /// {
    ///     Black, White, Red, Blue
    /// }
    /// 
    /// var integers = new object[] { 1, 2, 3, 4 };
    /// 
    /// var colors = integers.AsEnumArray&lt;Colors&gt;();
    /// // colors is now an array of 'Colors', with one of each of the values:
    /// // colors[0] == Black
    /// // colors[1] == White
    /// // ...
    /// // colors[3] == Blue
    /// </code>
    /// 
    /// <code class="language-csharp hljs">
    /// public enum Colors
    /// {
    ///     Black, White, Red, Blue
    /// }
    /// 
    /// var texts = new object[] { "Red", "Black", "White" };
    /// var colors = texts.AsEnumArray&lt;Colors&gt;();
    /// // colors[1] == Black
    /// </code>
    /// </example>
    /// <returns>An array of Enum Type</returns>
    public static TEnum[] AsEnumArray<TEnum>(this object[] objects) where TEnum : struct, IComparable, IFormattable, IConvertible
    {
        if (objects.IsNot()) return default;

        //TODO: consider "ToString()" each object, and "ToEnum<TEnum>()" each string...
        //but that would be a new method: ToEnumArray()

        var type = objects[0].GetType();

        if (type == SystemType.IntType)
            return objects.Cast<TEnum>().ToArray();
        else if (type == SystemType.StringType)
            return objects.Select(x => x.ToString().ToEnum<TEnum>()).ToArray();

        throw new Exception("Not supporting conversion of " + type.Name + "-array to the Enum");
    }
}
