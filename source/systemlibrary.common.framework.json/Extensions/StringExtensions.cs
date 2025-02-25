using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

using SystemLibrary.Common.Framework;

/// <summary>
/// This class contains extension methods for string
/// <para>StringExtensions exists in the global namespace</para>
/// </summary>
/// <example>
/// <code>
/// var result = "Hello world".Is()
/// // result is 'true'
/// </code>
/// <code>
/// var result = "".IsNot();
/// // result is 'true'
/// </code>
/// </example>
public static partial class StringExtensions
{
    internal static ConcurrentDictionary<int, MemberInfo[]> TypeEnumStaticMembers;
    static StringExtensions()
    {
        TypeEnumStaticMembers = new ConcurrentDictionary<int, MemberInfo[]>();
    }

    /// <summary>
    /// Return a part of the json as T
    /// <para>Searches through the json looking for a property that has the same name as T type, and outputs T</para>
    /// <para>Supports taking a 'search property path' seperated by a forward slash to the leaf property you want to convert to T, case insensitive</para>
    /// <para>Throws if a node towards the leaf is not found when specifying a 'search property path'</para>
    /// Throws if json has invalid formatted json text, does not throw on null/blank
    /// </summary>
    /// <typeparam name="T">A class or list/array of a class
    /// 
    /// If T is a list or array and no 'findPropertySearchPath' is specified, the Searcher appends an 's' as suffix
    /// 
    /// For instance List&lt;User&gt; will search for a property 'users', case insensitive and 's' is appended
    /// </typeparam>
    /// <param name="json">Json formatted string</param>
    /// <param name="findPropertySearchPath">
    /// Name of the property that will be deserialized as T
    /// 
    /// Example: root/property1/property2/leaf where 'leaf' will be deserialized as T
    /// </param>
    /// <example>
    /// <code class="language-csharp hljs">
    /// // Assume json string stored in a C# variable named 'data':
    /// var data = "{
    ///     "users" [
    ///         ...
    ///     ]
    /// }";
    /// var users = data.JsonPartial&lt;List&lt;User&gt;&gt;();
    /// // When a 'property name' is not given as first argument, it uses the type name in the following manner:
    /// // 1. Takes the type name, or generic type name, in our case 'User'
    /// // 2. If type is a List or Array, it adds a plural 's', so now we have 'Users'
    /// // 3. It lowers first letter to match camel casing as thats the "norm", so now we have 'users'
    /// 
    /// // You could also pass in "users" manually if you wanted, result is the same:
    /// // var users = data.JsonPartial&lt;List&lt;User&gt;&gt;("users");
    /// 
    /// // Conclusion: 
    /// // Automatic finding the property name if not specified as first argument
    /// // It returns first "users" match in the json, no matter how deep it is
    /// 
    /// //Assume json string stored in a C# variable named 'data':
    /// var data = "{
    ///     "users" [
    ///         ...
    ///     ],
    ///     "deactivated": {
    ///         "users": [
    ///             ...
    ///         ]
    ///     }
    /// }";
    /// var users = data.JsonPartial&lt;List&lt;User&gt;&gt;("deactivated/users");
    /// // Searches for a property "deactivated" anywhere in the json, then anywhere inside that, a "users" property
    /// 
    /// 
    /// // Assume json string stored in a C# variable named 'data':
    /// var data = "{
    ///     "text": "hello world",
    ///     "employees": [
    ///         {
    ///             "hired": [
    ///                ...
    ///             ],
    ///             "fired": [
    ///                 ...
    ///             ]
    ///         }
    ///     ],
    /// }";
    /// 
    /// var users = data.JsonPartial&lt;List&lt;User&gt;&gt;("fired");
    /// // Searches for a property anywhere in the json named "fired"
    /// </code>
    /// </example>
    /// <returns>Returns T or null if the leaf property do not exist</returns>
    public static T JsonPartial<T>(this string json, string findPropertySearchPath = null, JsonSerializerOptions options = null)
    {
        return PartialJsonSearcher.Search<T>(json, findPropertySearchPath, options);
    }

    /// <summary>
    /// Convert string formatted json to object T
    /// <para>Default options are: </para>
    /// <para>- case insensitive deserialization</para>
    /// <para>- allows trailing commas</para>
    /// Throws exception if json has invalid formatted json text, does not throw on null/blank
    /// </summary>
    /// <example>
    /// <code class="language-csharp hljs">
    /// class User {
    ///     public string FirstName;
    ///     public int Age { get; set;}
    /// }
    /// var json = "{
    ///     "firstName": 'hello',
    ///     "age": 10
    /// }";
    /// 
    /// var user = json.Json&lt;User&gt;();
    /// // NOTE: Naming is camelCase'd in json, but still matched (case insensitive) during deserialization by default
    /// </code>
    /// </example>
    /// <returns>Returns T or null if json is null or empty</returns>
    public static T Json<T>(this string json, JsonSerializerOptions options = null, bool transformUnicodeCodepoints = false) where T : class
    {
        if (json.IsNot()) return default;

        options = JsonSerializerOptionsInstance.Current(options);

        if (transformUnicodeCodepoints)
            json = json.TranslateUnicodeCodepoints();

        return JsonSerializer.Deserialize<T>(json, options);
    }

    /// <summary>
    /// Convert string formatted json to object T with your additional JsonConverters
    /// <para>Throws exception if json has invalid formatted json text, does not throw on null/blank</para>
    /// </summary>
    /// <example>
    /// <code class="language-csharp hljs">
    /// class User {
    ///     public string FirstName;
    ///     public int Age { get; set;}
    /// }
    /// 
    /// class CustomConverter : JsonConverter...
    /// 
    /// var json = "{
    ///     "firstName": 'hello',
    ///     "age": 10
    /// }";
    /// 
    /// var user = json.Json&lt;User&gt;(new CustomConverter());
    /// // NOTE: Naming is camelCase'd in json, but still matched (case insensitive) during deserialization by default
    /// </code>
    /// </example>
    /// <returns>Returns T or null if json is null or empty</returns>
    public static T Json<T>(this string json, params JsonConverter[] converters) where T : class
    {
        if (json.IsNot()) return null;

        var options = JsonSerializerOptionsInstance.Current(null, converters);

        return JsonSerializer.Deserialize<T>(json, options);
    }

    /// <summary>
    /// Check if input is a valid json beginning
    /// </summary>
    /// <example>
    /// var data = "Hello world";
    /// var isJson = data.IsJon(); // False
    /// </example>
    /// <returns>True or false</returns>
    public static bool IsJson(this string data)
    {
        if (data.IsNot()) return false;

        if (data.StartsWithAny("{", "[", " [", " {", "\r\n{", "\r\n[", "\n{", "\n[", "\n {", "\n [", "\t{", "\t["))
        {
            return true;
            //if (data.EndsWithAny("}", "]",
            //    "} ", "] ",
            //    "}\n", "]\n",
            //    "]" + Environment.NewLine, "}" + Environment.NewLine,
            //    "]\r\n", "}\r\n",
            //    "] \r\n", "} \r\n"
            //    ))
        }
        return false;
    }

    public static bool IsXml(this string data)
    {
        if (data.IsNot()) return false;

        return data.StartsWithAny("<", "\n<", "\r\n<", " <") && data.Contains(">");
    }
}
