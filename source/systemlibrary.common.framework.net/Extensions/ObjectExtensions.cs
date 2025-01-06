using System.Text.Json;
using System.Text.Json.Serialization;

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

    /// <summary>
    /// Convert object to its json string representation with option to camelCase properties
    /// </summary>
    /// <remarks>
    /// Uses built-in custom json converters for int, datetime, Enum, etc.
    /// - for instance Enum, with an EnumValue attribute, will be outputted as the EnumValue attribute and not the Enum.Key
    /// </remarks>
    /// <example>
    /// <code class="language-csharp hljs">
    /// // Assume camelCase argument true:
    /// class User {
    ///     public string FirstName { get;set; }
    /// }
    /// 
    /// var user = new User();
    /// user.FirstName = "Hello World";
    /// var json = user.Json();
    /// var contains = json.Contains("firstName") &amp;&amp; json.Contains("Hello World"); 
    /// // contains is True, note that Json() has formatted 'FirstName' to 'firstName' when going from C# model to json string
    /// </code>
    /// </example>
    /// <returns>Returns json string representation of input, or null if input was so</returns>
    public static string Json(this object obj, bool camelCase)
    {
        if (obj == null) return null;

        var options = JsonSerializerOptionsInstance.Current(null);

        if (camelCase)
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        else
            options.PropertyNamingPolicy = null;

        return JsonSerializer.Serialize(obj, options).TranslateUnicodeCodepoints();
    }

    /// <summary>
    /// Convert object to its json string representation with option to pass Custom JsonConverters
    /// </summary>
    /// <remarks>
    /// Uses built-in custom json converters for int, datetime, Enum, etc.
    /// - for instance Enum, with an EnumValue attribute, will be outputted as the EnumValue attribute and not the Enum.Key
    /// </remarks>
    /// <example>
    /// <code class="language-csharp hljs">
    /// class User {
    ///     public string FirstName { get;set; }
    /// }
    /// 
    /// class CustomConverter : JsonConverter...
    /// 
    /// var user = new User();
    /// user.FirstName = "Hello World";
    /// var json = user.Json(new CustomConverter());
    /// var isTrue = json.Contains("FirstName") &amp;&amp; json.Contains("Hello World"); 
    /// // isTrue is 'True' as FirstName is not camelCased by default and 'Hello World' is its value
    /// </code>
    /// </example>
    /// <returns>Returns json string representation of input, or null if input was so</returns>
    public static string Json(this object obj, JsonSerializerOptions options = null, bool translateUnicodeCodepoints = false, params JsonConverter[] jsonConverters)
    {
        if (obj == null) return null;

        options = JsonSerializerOptionsInstance.Current(options, jsonConverters);

        if (!translateUnicodeCodepoints)
            return JsonSerializer.Serialize(obj, options);

        return JsonSerializer.Serialize(obj, options).TranslateUnicodeCodepoints();
    }

    /// <summary>
    /// Convert object to its json string representation with option to pass Custom JsonConverters
    /// </summary>
    /// <remarks>
    /// Uses built-in custom json converters for int, datetime, Enum, etc.
    /// - for instance Enum, with an EnumValue attribute, will be outputted as the EnumValue attribute and not the Enum.Key
    /// </remarks>
    /// <example>
    /// <code class="language-csharp hljs">
    /// class User {
    ///     public string FirstName { get;set; }
    /// }
    /// 
    /// class CustomConverter : JsonConverter...
    /// 
    /// var user = new User();
    /// user.FirstName = "Hello World";
    /// var json = user.Json(new CustomConverter());
    /// var isTrue = json.Contains("FirstName") &amp;&amp; json.Contains("Hello World"); 
    /// // isTrue is 'True' as FirstName is not camelCased by default and 'Hello World' is its value
    /// </code>
    /// </example>
    /// <returns>Returns json string representation of input, or null if input was so</returns>
    public static string Json(this object obj, params JsonConverter[] jsonConverters)
    {
        if (obj == null) return null;

        var options = JsonSerializerOptionsInstance.Current(null, jsonConverters);

        return JsonSerializer.Serialize(obj, options);
    }
}
