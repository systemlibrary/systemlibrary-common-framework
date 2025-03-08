using System.Collections.Concurrent;
using System.Dynamic;
using System.Reflection;

namespace SystemLibrary.Common.Framework.Extensions;

/// <summary>
/// This class contains extension methods for Dynamic objects
/// </summary>
/// <remarks>
/// Current version of C# does not allow extension methods on 'dynamic', hence these are static methods
/// </remarks>
public static class DynamicExtensions
{
    internal static ConcurrentDictionary<int, PropertyInfo[]> MergeProperties;

    static DynamicExtensions()
    {
        MergeProperties = new ConcurrentDictionary<int, PropertyInfo[]>();
    }

    /// <summary>
    /// Merge dynamic objects into a one new dynamic object
    /// 
    /// <para>The latter updating value takes precedence if both objects contains same property name</para>
    /// </summary>
    /// <param name="source">An anonymous/dynamic object</param>
    /// <param name="updates">One or more anonymous/dynamic objects, but if error occurs, cast the 'updating' objects to an actual object before invoking this method</param>
    /// <returns>Returns a new dynamic object with the merge result, can be casted to IDictionary&lt;string, object&gt;</returns>
    /// <example>
    /// <code class="language-csharp hljs">
    /// var a = new
    /// {
    ///     firstName = "world",
    ///     age = 1
    /// };
    /// var b = new
    /// {
    ///     firstName = "hello",
    ///     Age = 2
    /// };
    /// var c = new 
    /// {
    ///    Age = 10   
    /// }
    ///
    /// var d = DynamicExtensions.Merge(a, b, c);
    /// 
    /// // d contains 3 properties: firstName, age, Age
    /// // d's property values are now: hello, 1, 10
    /// // var dictionary = d as IDictionary&lt;string, object&gt;
    /// // d can be cast to dictionary
    /// // dictionary["Age"] will return 10
    /// </code>
    /// </example>
    public static dynamic Merge(dynamic source, params object[] updates)
    {
        if (source == null && (updates == null || updates.Length == 0)) return null;

        var dictionary = new ExpandoObject() as IDictionary<string, object>;

        if (source != null)
        {
            var type = (Type)source.GetType();

            var properties = MergeProperties.Cache(type, () =>
            {
                return type.GetProperties(BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance);
            });

            foreach (PropertyInfo property in properties)
                if (property.CanRead)
                    dictionary[property.Name] = property.GetValue(source);
        }

        if (updates.Is())
        {
            foreach (var update in updates)
            {
                var type = update.GetType();

                var properties = MergeProperties.Cache(type, () =>
                {
                    return type.GetProperties(BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance);
                });

                foreach (PropertyInfo property in properties)
                    if (property.CanRead)
                        dictionary[property.Name] = property.GetValue(update);
            }
        }

        return dictionary as ExpandoObject;
    }
}
