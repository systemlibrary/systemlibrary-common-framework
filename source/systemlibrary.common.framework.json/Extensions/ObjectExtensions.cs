using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace SystemLibrary.Common.Framework.Extensions;

/// <summary>
/// This class contains extension methods on Object
/// </summary>
public static class ObjectExtensions
{
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

    public static string Xml(this object obj)
    {
        if (obj == null) return null;

        var serializer = new XmlSerializer(obj.GetType());

        using var writer = new StringWriter();
        
        serializer.Serialize(writer, obj);

        return writer.ToString();
    }
}
