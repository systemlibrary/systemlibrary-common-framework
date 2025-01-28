using System.Collections.Concurrent;

namespace SystemLibrary.Common.Framework.Extensions;

/// <summary>
/// Concurrent dictionary extensions
/// </summary>
public static class ConcurrentDictionaryExtensions
{
    /// <summary>
    /// Get T from a static concurrent dictionary based on a key 'int' or adds it to dictionary before returnal
    /// </summary>
    /// <remarks>
    /// The static concurrent dictionary lives as long as application runs
    /// <para>You are responsible for when to instantiate the dictionary</para>
    /// Item limit per dictionary is set to 50.000 items, if reached the dictionary is cleaned and starts caching over again
    /// </remarks>
    /// <returns>Returns T either from Cache or from the Method</returns>
    public static T Cache<T>(this ConcurrentDictionary<int, T> dictionary, int key, Func<T> getItem)
    {
        if (dictionary == null)
        {
            return getItem();
        }

        if (!dictionary.TryGetValue(key, out var result))
        {
            if (dictionary.Count > 50000)
            {
                dictionary.Clear();
            }
            result = getItem();

            dictionary.TryAdd(key, result);
        }

        return result;
    }

    /// <summary>
    /// Get T from a static concurrent dictionary based on a key 'Type' or adds it to dictionary before returnal
    /// </summary>
    /// <remarks>
    /// The static concurrent dictionary lives as long as application runs
    /// <para>You are responsible for when to instantiate the dictionary</para>
    /// <para>Item limit per dictionary is set to 50.000 items, if reached the dictionary is cleaned and starts caching over again</para>
    /// </remarks>
    /// <returns>Returns T either from Cache or from the Method</returns>
    public static T Cache<T>(this ConcurrentDictionary<int, T> dictionary, Type type, Func<T> getItem)
    {
        if (dictionary == null)
        {
            return getItem();
        }

        var hashCode = type.GetHashCode();

        if (!dictionary.TryGetValue(hashCode, out var result))
        {
            if (dictionary.Count > 50000)
            {
                dictionary.Clear();
            }
            result = getItem();

            dictionary.TryAdd(hashCode, result);
        }

        return result;
    }

    /// <summary>
    /// Get T from a static concurrent dictionary based on a key 'string' or adds it to dictionary before returnal
    /// </summary>
    /// <remarks>
    /// The static concurrent dictionary lives as long as application runs
    /// <para>You are responsible for when to instantiate the dictionary</para>
    /// Item limit per dictionary is set to 50.000 items, if reached the dictionary is cleaned and starts caching over again
    /// </remarks>
    /// <returns>Returns T either from Cache or from the Method</returns>
    public static T Cache<T>(this ConcurrentDictionary<string, T> dictionary, string key, Func<T> getItem)
    {
        if (dictionary == null)
            return getItem();

        if (key == null)
            return getItem();

        if (!dictionary.TryGetValue(key, out var result))
        {
            if (dictionary.Count > 50000)
            {
                dictionary.Clear();
            }
            result = getItem();

            dictionary.TryAdd(key, result);
        }

        return result;
    }
}
