namespace SystemLibrary.Common.Framework.Extensions;

/// <summary>
/// This class contains extension methods for Arrays
/// </summary>
public static class ArrayExtensions
{
    /// <summary>
    /// Add one array with another, resulting in a new array
    /// </summary>
    /// <example>
    /// <code>
    /// var arr = new int [] { 2 };
    /// var arr2 = new int [] { 1 };
    /// var arr3 = arr.Add(arr2);
    /// // arr3 now contains { 2, 1 }, in that order
    /// </code>
    /// </example>
    /// <returns>Returns null or a new array</returns>
    public static T[] Add<T>(this T[] current, params T[][] additional)
    {
        return Add(current, null, additional);
    }

    /// <summary>
    /// Add one array with another, resulting in a new array
    /// <para>- Pass a predicate method to filter out values</para>
    /// </summary>
    /// <example>
    /// <code>
    /// var arr = new int [] { 3,4 };
    /// var arr2 = new int [] { 1,2,3 };
    /// var arr3 = arr.Add((i) => i > 1, arr2);
    /// // arr3 now contains { 2,3,3,4 }, in that order, note the duplicated values
    /// </code>
    /// </example>
    /// <returns>Returns null if everything is null, else a new array of items</returns>
    public static T[] Add<T>(this T[] current, Func<T, bool> predicate, params T[][] additional)
    {
        if (additional == null ||
            additional.Length == 0 ||
            (additional.Length == 1 && additional[0] == null)) return current;

        var sum = current?.Length ?? 0;

        foreach (var add in additional)
            sum += add?.Length ?? 0;

        var result = (T[])Activator.CreateInstance(typeof(T[]), sum);

        var i = 0;

        if (current != null)
        {
            foreach (var v in current)
            {
                if (predicate == null || predicate(v))
                {
                    result[i] = v;
                    i++;
                }
            }
        }

        foreach (var add in additional)
        {
            if (add != null)
            {
                foreach (var v in add)
                {
                    if (predicate == null || predicate(v))
                    {
                        result[i] = v;
                        i++;
                    }
                }
            }
        }

        if (i != sum)
        {
            return result.Take(i).ToArray();
        }

        return result;
    }
}
