using System.Collections;

namespace SystemLibrary.Common.Framework.Extensions;

/// <summary>
/// This class contains extension methods for IEnumerables
/// </summary>
public static class IEnumerableExtensions
{
    /// <summary>
    /// Select items grouped by a property of your choice
    /// </summary>
    /// <example>
    /// <code class="language-csharp hljs">
    /// class Car 
    /// {
    ///     public string Name;
    /// }
    /// 
    /// var car1 = new Car { Name = "Vehicle" }
    /// var car2 = new Car { Name = "Vehicle" }
    /// 
    /// var list = new List&lt;Car&gt; { car1, car2 }
    /// 
    /// var list = list.DistinctBy(x => x.Name).ToList();
    /// // list contains now 1 car
    /// </code>
    /// </example>
    /// <returns>IEnumerable filtered on the property of your choice</returns>
    public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> items, Func<T, TKey> selector)
        where T : class
    {
        return items.GroupBy(selector).Select(grp => grp.FirstOrDefault());
    }

    /// <summary>
    /// Check if the IEnumerable contains 'value'
    /// 
    /// Does not throw on null
    /// </summary>
    /// <remarks>
    /// Uses Linq.Contains() internally, but does not throw on null
    /// </remarks>
    /// <example>
    /// <code class="language-csharp hljs">
    ///  var texts = new string[] { "Hello", "World" };
    ///  var has = texts.Has("Abc");
    ///  //has is False
    /// </code>
    /// </example>
    /// <returns>Returns true or false</returns>
    public static bool Has<T>(this IEnumerable<T> enumerable, T value) where T : IComparable, IConvertible
    {
        if (enumerable == null) return false;

        if (value == null) return false;

        return enumerable.Contains(value);
    }

    /// <summary>
    /// Check if the enumerable contains 'value', does not throw if value is null
    /// </summary>
    /// <remarks>Uses Linq.Contains() internally</remarks>
    /// <example>
    /// <code class="language-csharp hljs">
    /// var users = new List&lt;User&gt;();
    /// var user = new User();
    /// 
    /// users.Add(user);
    /// 
    /// var result = users.Has(user);
    /// // result is true, the list contains that specific user object
    /// </code>
    /// </example>
    /// <returns>Returns true or false</returns>
    public static bool Has<T>(this IEnumerable<T> enumerable, object value) where T : class
    {
        if (enumerable == null) return false;

        if (value == null) return false;

        if (value is T t)
            return enumerable.Contains(t);

        return false;
    }

    /// <summary>
    /// Checks if the Enumerable exists and has at least 1 item
    /// </summary>
    /// <remarks>
    /// Does not throw exception on null
    /// </remarks>
    /// <example>
    /// <code class="language-csharp hljs">
    /// var list = new List&lt;string&gt;();
    /// 
    /// var result = list.IsNot();
    /// 
    /// // result is true as the list contains 0 items
    /// 
    /// List&lt;string&gt; list = null;
    /// var result = list.IsNot();
    /// // result is true as the list currently is null
    /// </code>
    /// </example>
    /// <returns>True or false</returns>
    public static bool IsNot<T>(this IEnumerable<T> enumerable)
    {
        if (enumerable == null) return true;

        if (enumerable is ICollection iCollection)
        {
            if (iCollection.Count == 0) return true;

            if (iCollection.Count == 1 && iCollection is IList list)
                return list[0] == null;

            return false;
        }

        return !enumerable.Any();
    }

    /// <summary>
    /// Checks if the Enumerable exists and has at least 1 item
    /// </summary>
    /// <remarks>
    /// Does not throw exception on null
    /// </remarks>
    /// <example>
    /// <code class="language-csharp hljs">
    /// var list = new List&lt;string&gt;();
    /// list.Add("hello world");
    /// 
    /// var result = list.Is();
    /// 
    /// // result is true as the list contains 1 item
    /// 
    /// List&lt;string&gt; list = null;
    /// var result = list.Is();
    /// // result is false as the list is null
    /// </code>
    /// </example>
    /// <returns>True or false</returns>
    public static bool Is<T>(this IEnumerable<T> enumerable)
    {
        if (enumerable == null) return false;

        if (enumerable is ICollection iCollection)
            return iCollection.Count > 0;

        return enumerable.Any();
    }
}