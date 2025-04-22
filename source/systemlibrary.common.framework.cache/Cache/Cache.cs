using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

using SystemLibrary.Common.Framework.Extensions;
using SystemLibrary.Common.Framework.Licensing;

namespace SystemLibrary.Common.Framework.App;

/// <summary>
/// Caching for applications
/// <para>Default duration is 3 minutes</para>
/// Try using auto-generating cache keys, which differentiate caching down to user roles.
/// <para>- Cache things per user, by userId/email? Create your own cacheKey</para>
/// <para>'Ignore' means the function will always be invoked directly, bypassing the cache entirely.</para>
/// Skip options:
/// <para>- skipWhenAuthenticated, false by default</para>
/// - skipWhenAdmin, true by default
/// <para>The user must belong to one of the following case-sensitive roles: Admin, Admins, Administrator, Administrators, WebAdmins, CmsAdmins, admin, admins, administrator, administrators.</para>
/// - skipWhen, your own condition, must return True to skip
/// </summary>
/// <remarks>
/// Cache is configured to a max capacity of 320.000 items, divided by 8 cache containers, where any item added takes up 1 size
/// <para>Each container is configured to a max capacity of 40.000 items, once reached 33% of the oldest are removed, ready to be GC'ed</para>
/// A null value is never added to cache
/// <para>Overwrite default cache configurations in appsettings.json:</para>
/// - duration: 180, minimum 1
/// <para>- fallbackDuration: 300, set to 0 or negative to disable fallback cache globally</para>
/// - containerSizeLimit: 60000, minimum 10
/// <para>Auto-generating cache key adds namespace, class, method, method-scoped variables of types such as bool, string, int, datetime, enum and few others</para>
/// <para>- If a method-scoped variable is a class, its public members of same types are also appended as cacheKey</para>
/// - IsAuthenticated is always appended to cacheKey
/// <para>- Claim 'role', 'Role' and RoleClaimType if found, is always appended to cacheKey</para>
/// - Always adds built-in prefix
/// </remarks>
/// <example>
/// Configure the cache in appsettings.json, heres the default:
/// <code>
/// {
///     "systemLibraryCommonFramework": {
///         "cache" { 
///             "duration": 180,
///             "fallbackDuration": 300,
///             "containerSizeLimit": 60000
///         }
///     }
/// }
/// </code>
/// Use cache:
/// <code>
/// using SystemLibrary.Common.Framework.App;
/// 
/// var cacheKey = "key";
/// var item = Cache.Get(cacheKey);
/// // null if not in cache
/// </code>
/// </example>
public static partial class Cache
{
    internal static ConcurrentDictionary<int, FieldInfo[]> GenerateCacheKeyFields;
    internal static ConcurrentDictionary<int, FieldInfo[]> GenerateCacheKeyValueTypeFields;
    internal static ConcurrentDictionary<int, PropertyInfo[]> GenerateCacheKeyValueTypeProperties;
    internal static string PrevCacheKey;

    static IPrincipal Principal()
    {
        return HttpContextInstance.Current?.User;
    }

    static IMemoryCache[] cache;
    static IMemoryCache[] cacheFallback;
    static int MaxCacheContainers = 8; // Must be power of 2 as we use bit operator to calc index

    static Cache()
    {
        GenerateCacheKeyFields = new ConcurrentDictionary<int, FieldInfo[]>();
        GenerateCacheKeyValueTypeFields = new ConcurrentDictionary<int, FieldInfo[]>();
        GenerateCacheKeyValueTypeProperties = new ConcurrentDictionary<int, PropertyInfo[]>();

        cache = new IMemoryCache[MaxCacheContainers];

        if (FallbackDurationConfig > 0)
            cacheFallback = new IMemoryCache[MaxCacheContainers];

        for (int i = 0; i < MaxCacheContainers; i++)
        {
            MemoryCacheOptions options = new MemoryCacheOptions();
            options.ExpirationScanFrequency = TimeSpan.FromSeconds(90 + Randomness.Int(4) + (i * 5));
            options.SizeLimit = ContainerSizeLimitConfig;
            options.CompactionPercentage = 0.33;

            options.TrackStatistics = false;
            options.TrackLinkedCacheEntries = false;

            cache[i] = new MemoryCache(options);

            if (FallbackDurationConfig > 0)
                cacheFallback[i] = new MemoryCache(options);
        }
    }

    /// <summary>
    /// Get item from Cache as T
    /// </summary>
    /// <remarks>
    /// CacheKey null or blank returns default without checking cache
    /// <para>This never checks fallback cache</para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var cacheKey = "helloworld";
    /// var data = Cache.Get&lt;string&gt;(cacheKey);
    /// </code>
    /// </example>
    /// <returns>Return item from cache if exists or default</returns>
    public static T Get<T>(string cacheKey)
    {
        if (cacheKey.IsNot()) return default;

        var cacheIndex = cacheKey.GetHashCode() & 7;

        if (cache[cacheIndex] == null) return default;

        var cached = cache[cacheIndex].Get(cacheKey);

        return cached == null ? default : (T)cached;
    }

    /// <summary>
    /// Add item to cache
    /// </summary>
    /// <remarks>
    /// A null value is never added to cache
    /// </remarks>
    /// <param name="cacheKey">CacheKey to set item as, if null or empty this does nothing</param>
    /// <param name="duration">Defaults to 180 seconds</param>
    public static void Set<T>(string cacheKey, T item, CacheDuration duration = default)
    {
        if (cacheKey.IsNot())
            return;

        var cacheIndex = cacheKey.GetHashCode() & 7;

        Insert(cacheIndex, cacheKey, item, duration);
    }

    /// <summary>
    /// Try get item from Cache as T
    /// <para>If getItem throws, the exception is logged as 'Error'</para>
    /// </summary>
    /// <remarks>
    /// <para>A null value is never added to cache</para>
    /// Default duration is 200 seconds
    /// <para>'Ignore' means the function will always be invoked directly, bypassing the cache entirely.</para>
    /// Skip options:
    /// <para>- skipWhenAuthenticated, false by default</para>
    /// - skipWhenAdmin, true by default
    /// <para>  The user must belong to one of the following case-sensitive roles: Admin, Admins, Administrator, Administrators, WebAdmins, CmsAdmins, admin, admins, administrator, administrators.</para>
    /// - skipWhen, your own condition, must return True to skip
    /// </remarks>
    /// <param name="cacheKey">"" to use auto-generating of cacheKey, null to always skip cache</param>
    /// <param name="condition">Add to cache only if condition is true, for instance: data?.Count > 0</param>
    /// <param name="skipWhenAuthenticated">Skip cache for any user that is authenticated through the current HttpContext.User instance</param>
    /// <param name="skipWhenAdmin">Skip cache for any user that is authenticated through the current HttpContext.User and is in any role: Admin, Admins, Administrator, Administrators, WebAdmins, CmsAdmins, admin, admins, administrator, administrators</param>
    /// <param name="skipWhen">Implement your own logic for when to skip cache, let it return true on your conditions to avoid caching</param>
    /// <example>
    /// <code>
    /// var cacheKey = "key";
    /// 
    /// var data = Cache.TryGet&lt;string&gt;(cacheKey, () => throw new Exception("does not crash application"));
    /// 
    /// // Exception is logged through your ILogWriter implementation
    /// </code>
    /// </example>
    /// <returns>Returns T from cache or from getItem. If getItem throws, the exception is logged as error and default is returned</returns>
    public static T TryGet<T>(string cacheKey, Func<T> getItem, CacheDuration duration = default, Func<T, bool> condition = null, bool skipWhenAuthenticated = false, bool skipWhenAdmin = true, Func<bool> skipWhen = null)
    {
        try
        {
            return Get(getItem, cacheKey, duration, condition, skipWhenAuthenticated, skipWhenAdmin, skipWhen);
        }
        catch (Exception ex)
        {
            Log.Error(ex);

            return default;
        }
    }

    /// <summary>
    /// Try get item from Cache as T using auto-generated cache key
    /// <para>Logs exception if getItem() throws</para>
    /// </summary>
    /// <remarks>
    /// <para>A null value is never added to cache</para>
    /// Default duration is 200 seconds
    /// <para>'Ignore' means the function will always be invoked directly, bypassing the cache entirely.</para>
    /// Skip options:
    /// <para>- skipWhenAuthenticated, false by default</para>
    /// - skipWhenAdmin, true by default
    /// <para>  The user must belong to one of the following case-sensitive roles: Admin, Admins, Administrator, Administrators, WebAdmins, CmsAdmins, admin, admins, administrator, administrators.</para>
    /// - skipWhen, your own condition, must return True to skip
    /// </remarks>
    /// <param name="condition">Add to cache only if condition is true, for instance: data?.Count > 0</param>
    /// <param name="skipWhenAuthenticated">Skip cache for any user that is authenticated through the current HttpContext.User instance</param>
    /// <param name="skipWhenAdmin">Skip cache for any user that is authenticated through the current HttpContext.User and is in any role: Admin, Admins, Administrator, Administrators, WebAdmins, CmsAdmins, admin, admins, administrator, administrators</param>
    /// <param name="skipWhen">Implement your own logic for when to skip cache, let it return true on your conditions to avoid caching</param>
    /// <example>
    /// <code>
    /// var data = Cache.TryGet&lt;string&gt;(() => throw new Exception("does not crash application"));
    /// // Exception is logged through your ILogWriter implementation
    /// </code>
    /// </example>
    /// <returns>Returns T from cache or from getItem. If getItem throws, the exception is logged as error and default is returned</returns>
    public static T TryGet<T>(Func<T> getItem, CacheDuration duration, Func<T, bool> condition = null, bool skipWhenAuthenticated = false, bool skipWhenAdmin = true, Func<bool> skipWhen = null)
    {
        try
        {
            return Get(getItem, "", duration, condition, skipWhenAuthenticated, skipWhenAdmin, skipWhen);
        }
        catch (Exception ex)
        {
            Log.Error(ex);

            return default;
        }
    }

    /// <summary>
    /// Try get item from Cache as T using auto-generated cache key
    /// <para>Logs exception if getItem() throws</para>
    /// </summary>
    /// <remarks>
    /// <para>A null value is never added to cache</para>
    /// Default duration is 200 seconds
    /// <para>'Ignore' means the function will always be invoked directly, bypassing the cache entirely.</para>
    /// Skip options:
    /// <para>- skipWhenAuthenticated, false by default</para>
    /// - skipWhenAdmin, true by default
    /// <para>  The user must belong to one of the following case-sensitive roles: Admin, Admins, Administrator, Administrators, WebAdmins, CmsAdmins, admin, admins, administrator, administrators.</para>
    /// - skipWhen, your own condition, must return True to skip
    /// </remarks>
    /// <param name="cacheKey">"" to use auto-generating of cacheKey, null to always skip cache</param>
    /// <param name="condition">Add to cache only if condition is true, for instance: data?.Count > 0</param>
    /// <param name="skipWhenAuthenticated">Skip cache for any user that is authenticated through the current HttpContext.User instance</param>
    /// <param name="skipWhenAdmin">Skip cache for any user that is authenticated through the current HttpContext.User and is in any role: Admin, Admins, Administrator, Administrators, WebAdmins, CmsAdmins, admin, admins, administrator, administrators</param>
    /// <param name="skipWhen">Implement your own logic for when to skip cache, let it return true on your conditions to avoid caching</param>
    /// <example>
    /// <code>
    /// var data = Cache.TryGet&lt;string&gt;(() => throw new Exception("does not crash application"));
    /// 
    /// // Exception is logged through your ILogWriter implementation
    /// </code>
    /// </example>
    /// <returns>Returns T from cache or from getItem. If getItem throws, the exception is logged as error and default is returned</returns>
    public static T TryGet<T>(Func<T> getItem, string cacheKey = "", CacheDuration duration = default, Func<T, bool> condition = null, bool skipWhenAuthenticated = false, bool skipWhenAdmin = true, Func<bool> skipWhen = null)
    {
        try
        {
            return Get(getItem, cacheKey, duration, condition, skipWhenAuthenticated, skipWhenAdmin, skipWhen);
        }
        catch (Exception ex)
        {
            Log.Error(ex);

            return default;
        }
    }

    /// <summary>
    /// Get item from Cache as T
    /// </summary>
    /// <remarks>
    /// <para>A null value is never added to cache</para>
    /// Throws exception if getItem can throw
    /// <para>Default duration is 200 seconds</para>
    /// <para>'Ignore' means the function will always be invoked directly, bypassing the cache entirely.</para>
    /// Skip options:
    /// <para>- skipWhenAuthenticated, false by default</para>
    /// - skipWhenAdmin, true by default
    /// <para>  The user must belong to one of the following case-sensitive roles: Admin, Admins, Administrator, Administrators, WebAdmins, CmsAdmins, admin, admins, administrator, administrators.</para>
    /// - skipWhen, your own condition, must return True to skip
    /// </remarks>
    /// <param name="cacheKey">"" to use auto-generating of cacheKey, null to always skip cache</param>
    /// <param name="condition">Add to cache only if condition is true, for instance: data?.Count > 0</param>
    /// <param name="skipWhenAuthenticated">Skip cache for any user that is authenticated through the current HttpContext.User instance</param>
    /// <param name="skipWhenAdmin">Skip cache for any user that is authenticated through the current HttpContext.User and is in any role: Admin, Admins, Administrator, Administrators, WebAdmins, CmsAdmins, admin, admins, administrator, administrators</param>
    /// <param name="skipWhen">Implement your own logic for when to skip cache, let it return true on your conditions to avoid caching</param>
    /// <code>
    /// class CarService
    /// {
    ///     public string GetCars() 
    ///     {
    ///         var cacheKey = "helloworld";
    ///         
    ///         return Cache.Get&lt;string&gt;(cacheKey, () => {
    ///             return Client.Get&lt;string&gt;("https://systemlibrary.com/api/cars?top=1");
    ///         },
    ///         TimeSpan.FromSeconds(5));
    ///     }
    /// }
    /// </code>
    /// <returns>Returns T from cache or from getItem, or throws if getItem throws</returns>
    public static T Get<T>(string cacheKey, Func<T> getItem, CacheDuration duration = default, Func<T, bool> condition = null, bool skipWhenAuthenticated = false, bool skipWhenAdmin = true, Func<bool> skipWhen = null)
    {
        return Get(getItem, cacheKey, duration, condition, skipWhenAuthenticated, skipWhenAdmin, skipWhen);
    }

    /// <summary>
    /// Get item from Cache as T using auto-generated cache key
    /// </summary>
    /// <remarks>
    /// <para>A null value is never added to cache</para>
    /// Throws exception if getItem can throw
    /// <para>Default duration is 200 seconds</para>
    /// <para>'Ignore' means the function will always be invoked directly, bypassing the cache entirely.</para>
    /// Skip options:
    /// <para>- skipWhenAuthenticated, false by default</para>
    /// - skipWhenAdmin, true by default
    /// <para>  The user must belong to one of the following case-sensitive roles: Admin, Admins, Administrator, Administrators, WebAdmins, CmsAdmins, admin, admins, administrator, administrators.</para>
    /// - skipWhen, your own condition, must return True to skip
    /// </remarks>
    /// <param name="condition">Add to cache only if condition is true, for instance: data?.Count > 0</param>
    /// <param name="skipWhenAuthenticated">Skip cache for any user that is authenticated through the current HttpContext.User instance</param>
    /// <param name="skipWhenAdmin">Skip cache for any user that is authenticated through the current HttpContext.User and is in any role: Admin, Admins, Administrator, Administrators, WebAdmins, CmsAdmins, admin, admins, administrator, administrators</param>
    /// <param name="skipWhen">Implement your own logic for when to skip cache, let it return true on your conditions to avoid caching</param>
    /// <code>
    /// class CarService
    /// {
    ///     public string GetCars() 
    ///     {
    ///         var cacheKey = "helloworld";
    ///         
    ///         return Cache.Get&lt;string&gt;(cacheKey, () => {
    ///             return Client.Get&lt;string&gt;("https://systemlibrary.com/api/cars?top=1");
    ///         },
    ///         TimeSpan.FromSeconds(5));
    ///     }
    /// }
    /// </code>
    /// <returns>Returns T from cache or from getItem, or throws if getItem throws</returns>
    public static T Get<T>(Func<T> getItem, CacheDuration duration, Func<T, bool> condition = null, bool skipWhenAuthenticated = false, bool skipWhenAdmin = true, Func<bool> skipWhen = null)
    {
        return Get(getItem, "", duration, condition, skipWhenAuthenticated, skipWhenAdmin, skipWhen);
    }

    /// <summary>
    /// Get item from Cache as T using auto-generated cache key
    /// </summary>
    /// <remarks>
    /// <para>A null value is never added to cache</para>
    /// Throws exception if getItem can throw
    /// <para>Default duration is 200 seconds</para>
    /// <para>'Ignore' means the function will always be invoked directly, bypassing the cache entirely.</para>
    /// Skip options:
    /// <para>- skipWhenAuthenticated, false by default</para>
    /// - skipWhenAdmin, true by default
    /// <para>  The user must belong to one of the following case-sensitive roles: Admin, Admins, Administrator, Administrators, WebAdmins, CmsAdmins, admin, admins, administrator, administrators.</para>
    /// - skipWhen, your own condition, must return True to skip
    /// </remarks>
    /// <param name="cacheKey">"" to use auto-generating of cacheKey, null to always skip cache</param>
    /// <param name="condition">Add to cache only if condition is true, for instance: data?.Count > 0</param>
    /// <param name="skipWhenAuthenticated">Skip cache for any user that is authenticated through the current HttpContext.User instance</param>
    /// <param name="skipWhenAdmin">Skip cache for any user that is authenticated through the current HttpContext.User and is in any role: Admin, Admins, Administrator, Administrators, WebAdmins, CmsAdmins, admin, admins, administrator, administrators</param>
    /// <param name="skipWhen">Implement your own logic for when to skip cache, let it return true on your conditions to avoid caching</param>
    /// <example>
    /// Simplest example:
    /// <code>
    /// var data = Cache.Get(() => {
    ///     return "hello world";
    /// });
    /// 
    /// //'data' is now 'hello world', if called multiple times within the default cache duration of 180 seconds, "hello world" is returned from the cache for all non-admin users
    /// </code>
    /// 
    /// Simplest example with cacheKey:
    /// <code>
    /// var cacheKey = "hello-world-key";
    /// var data = Cache.Get(() => {
    ///     return "hello world";
    /// },
    /// cacheKey: cacheKey);
    /// 
    /// //'data' is now 'hello world', if called multiple times within the default cache duration of 180 seconds, "hello world" is returned from the cache for all non-admin users
    /// </code>
    /// 
    /// Example with multiple options passed, and a condition that always fails:
    /// <code>
    /// var cacheKey = "hello-world-key";
    /// var data = Cache.Get(() => {
    ///         return "hello world";
    ///     },
    ///     cacheKey: cacheKey,
    ///     duration: TimeSpan.FromSeconds(1),
    ///     condition: x => x != "hello world",
    ///     skipWhenAuthenticated: false);
    /// 
    /// //'data' is equal to 'hello world', cache duration is 1 second, but it only adds the result to cache, if it is not equal to "hello world"
    /// // so in this scenario - "hello world" is never added to cache, and our function that returns "hello world" is always invoked
    /// </code>
    /// 
    /// Example without a cache key
    /// <code>
    /// class CarService
    /// {
    ///     public string GetCars() 
    ///     {
    ///         return Cache.Get&lt;string&gt;(() => {
    ///             return Client.Get&lt;string&gt;("https://systemlibrary.com/api/cars?top=1");
    ///         },
    ///         skipWhenAdmin: false);
    ///     }
    /// }
    /// // This caches top 1 cars for every user, even admins, as we set 'skipWhenAdmin' to False
    /// </code>
    /// 
    /// Example without a cache key and with 'external' variables
    /// <code>
    /// class CarService
    /// {
    ///     public string GetCars(int top = 10) 
    ///     {
    ///         var url = "https://systemlibrary.com/api/cars";
    ///         var urlQueryValue = "?filter=none";
    ///         
    ///         return Cache.Get&lt;string&gt;(() => {
    ///             return Client.Get&lt;string&gt;(url + urlQueryValue + " top=" + top);
    ///         });
    ///     }
    /// }
    /// 
    /// // Returns top 10 cars from the API, and adds result to cache (assumes not null) for a duration of 180 seconds by default
    /// // For simplicity, pretend an auto cache key looks like this: SLF%...
    /// 
    /// // Note: cache key is created with the outside variable "top", it is ".ToString'd", works on many types: bool, datetime, string, and simple POCO's with 1 depth level of properties/fields, not "class inside class" is not supported
    /// // Note: cache key for wether or not user is logged in is always appended so it always varies on "IsAuthenticated"
    /// </code>
    /// </example>
    /// <returns>Returns item from cache or getItem</returns>
    public static T Get<T>(Func<T> getItem, string cacheKey = "", CacheDuration duration = default, Func<T, bool> condition = null, bool skipWhenAuthenticated = false, bool skipWhenAdmin = true, Func<bool> skipWhen = null)
    {
        if (cacheKey == null)
            return getItem();

        if (SkipCache(skipWhenAuthenticated, skipWhenAdmin, skipWhen))
        {
            if (EnablePrometheusConfig && License.Gold())
                Metric.Inc("cache", "ignored");

            return getItem();
        }

        if (cacheKey == "")
        {
            cacheKey = CreateCacheKey(getItem, condition);

            // PrevCacheKey = cacheKey;
        }

        var cacheIndex = cacheKey.GetHashCode() & 7;

        // NOTE: cache[cacheIndex] is never null, but keeping it in case I've missed something
        if (cache[cacheIndex] == null)
            return getItem();

        var cached = cache[cacheIndex].Get(cacheKey);

        if (cached != null)
        {
            if (EnablePrometheusConfig && License.Gold())
                Metric.Inc("cache", "hit");

            return (T)cached;
        }

        object CacheFallbackLookup(Exception ex = null)
        {
            if (FallbackDurationConfig > 0)
            {
                if (License.Gold())
                {
                    if (cacheFallback[cacheIndex] == null) return null;

                    var cachedFallback = cacheFallback[cacheIndex].Get(cacheKey);

                    if (cachedFallback != null)
                    {
                        Log.Warning(new Exception("[Cache] Fallback match for key: " + cacheKey.MaxLength(48) + "...", ex));

                        return cachedFallback;
                    }
                }
            }
            return null;
        }

        try
        {
            cached = getItem();

            // If result from getItem still returns "null", let's lookup the cache fallback for next N min
            if (cached == null)
            {
                cached = CacheFallbackLookup();
                if (cached != null)
                {
                    if (EnablePrometheusConfig && License.Gold())
                        Metric.Inc("cache", "cache_miss_returned_fallback");

                    return (T)cached;
                }
            }
        }
        catch (Exception ex)
        {
            cached = CacheFallbackLookup(ex);
            if (cached != null)
            {
                if (EnablePrometheusConfig && License.Gold())
                    Metric.Inc("cache", "cache_exception_returned_fallback");

                return (T)cached;
            }

            if (EnablePrometheusConfig && License.Gold())
                Metric.Inc("cache", "cache_exception_thrown");

            throw ex;
        }

        if (cached != null && (condition == null || condition((T)cached)))
        {
            Insert(cacheIndex, cacheKey, cached, duration);

            if (EnablePrometheusConfig && License.Gold())
                Metric.Inc("cache", "miss");
        }
        else
        {
            if (EnablePrometheusConfig && License.Gold())
                Metric.Inc("cache", "ignored");
        }

        return (T)cached;
    }

    /// <summary>
    /// Ensures that the enclosed code block executes only once within the specified duration
    /// <para>Default break duration is 60 seconds</para>
    /// </summary>
    /// <remarks>
    /// Uses the stack frame to read current namespace and method as cache key, so max 1 invocation per function scope, else you must fill out the breakKey parameter too
    /// <para>- in the future it might support multiple...</para>
    /// <para>Multiple threads running at same time, will trigger this multiple times as we do not really 'lock'</para>
    /// </remarks>
    /// <param name="duration">The time span for which subsequent executions are prevented.</param>
    /// <example>
    /// <code>
    /// if(Cache.Lock(TimeSpan.FromSeconds(60), "send-email") 
    /// {
    ///     new Email(...).Send(); // Pseudo code
    ///     // Example: invoking this code 66 times, one time per second, where first invocation is one second from "now", will send two emails: one at second 1, and another at second 61
    /// }
    /// </code>
    /// </example>
    /// <returns>True if the block is allowed to execute; otherwise, false.</returns>
    public static bool Lock(CacheDuration duration = default, string lockKey = null)
    {
        try
        {
            var callee = new StackFrame(1).GetMethod();

            var cacheKey = nameof(SystemLibrary) + nameof(Cache) + nameof(Lock) + callee.DeclaringType?.Namespace + callee.DeclaringType?.Name + callee.Name + callee.IsStatic + callee.IsPublic + duration + lockKey;

            var cacheIndex = cacheKey.GetHashCode() & 7;

            if (cache[cacheIndex] == null) return false;

            var exists = cache[cacheIndex].Get<bool>(cacheKey);

            if (exists) return false;

            Insert(cacheIndex, cacheKey, true, duration);

            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Remove item from Cache
    /// </summary>
    /// <remarks>
    /// Does nothing if item do not exist in cache or if cacheKey is null/blank
    /// </remarks>
    /// <example>
    /// <code>
    /// var cacheKey = "hello world";
    /// Cache.Remove(cacheKey);
    /// </code>
    /// </example>
    public static void Remove(string cacheKey)
    {
        if (cacheKey.IsNot()) return;

        var cacheIndex = cacheKey.GetHashCode() & 7;

        if (cache[cacheIndex] == null) return;

        cache[cacheIndex].Remove(cacheKey);

        if (FallbackDurationConfig > 0)
            cacheFallback[cacheIndex].Remove(cacheKey);
    }

    /// <summary>
    /// Clear all entries found, which was set through this Cache class
    /// </summary>
    /// <remarks>
    /// Only entries set through either TryGet, Get or Set will be cleared
    /// <para>- other cache mechanisms that you are using are not touched</para>
    /// Clearing cache is not thread safe as it null's out the cache containers and recreates them all
    /// <para>- null checks exists before the cache containers are used, but it does not gurantee thread safety</para>
    /// </remarks>
    /// <example>
    /// <code>
    /// Cache.Clear();
    /// </code>
    /// </example>
    public static void Clear()
    {
        for (int i = 0; i < MaxCacheContainers; i++)
        {
            cache[i].Dispose();
            cache[i] = null;

            if (FallbackDurationConfig > 0)
            {
                cacheFallback[i].Dispose();
                cacheFallback[i] = null;
            }
        }

        for (int i = 0; i < MaxCacheContainers; i++)
        {
            MemoryCacheOptions options = new MemoryCacheOptions();
            options.ExpirationScanFrequency = TimeSpan.FromSeconds(90 + Randomness.Int(30));
            options.SizeLimit = ContainerSizeLimitConfig;
            options.CompactionPercentage = 0.33;
            cache[i] = new MemoryCache(options);

            if (FallbackDurationConfig > 0)
                cacheFallback[i] = new MemoryCache(options);
        }
    }

    static void Insert(int cacheIndex, string cacheKey, object item, CacheDuration duration)
    {
        if (cache[cacheIndex] == null) return;

        if (item == null)
        {
            Remove(cacheKey);
        }
        else
        {
            var cacheDuration = GetCacheDuration(duration);
            cache[cacheIndex].Set(cacheKey, item, new MemoryCacheEntryOptions()
            {
                AbsoluteExpiration = DateTime.Now.Add(cacheDuration),
                Size = 1,
            });

            if (FallbackDurationConfig > 0)
            {
                cacheFallback[cacheIndex].Set(cacheKey, item, new MemoryCacheEntryOptions()
                {
                    AbsoluteExpiration = DateTime.Now.Add(cacheDuration).AddSeconds(FallbackDurationConfig),
                    Size = 1,
                });
            }
        }
    }

    static string CreateCacheKey<T>(Func<T> getItem, Func<T, bool> condition)
    {
        // TODO: Rewrite into expressions for performance gains (benchmark it)
        /*
         Pseudo code:
        var lambda = getItem as LambdaExpression;
        var key = new StringBuilder("SLF%", capacity: 400);
        foreach (var parameter in lambda.Parameters)
        {
            var parameterValue = GetParameterValue(parameter, lambda);
            key.Append($"{parameter.Name}={parameterValue}");
        }
        static object GetParameterValue(ParameterExpression parameter, LambdaExpression lambda)
        {
            var compiledLambda = lambda.Compile(); // Lookup in internal cache dictionary 
            var result = compiledLambda.DynamicInvoke();
            // Log the result determine how it looks like...
            return result;
        }
        */

        var key = new StringBuilder("SLF%", capacity: 400);

        var getItemMethod = getItem.Method;

        key.Append(getItemMethod?.DeclaringType?.Namespace + getItemMethod.Name + getItemMethod?.DeclaringType?.Name + "");
        key.Append(getItemMethod.ReturnType?.Name ?? "");

        var target = getItem.Target;
        if (target != null)
        {
            void AppendString(object value, Type valueType)
            {
                if (value is string text)
                {
                    key.Append(text.GetCompressedKey());
                }
                else if (value is StringBuilder sb)
                {
                    key.Append(sb.GetCompressedKey());
                }
                else if (value is Guid g)
                {
                    key.Append(g.ToString("N"));
                }
                else if (value is DateTime dt)
                {
                    key.Append(dt.ToString("yyMMddHHmmss"));
                }
                else if (value is DateTimeOffset dto)
                {
                    key.Append(dto.ToString("yyMMddHHmmss"));
                }
                else if (IsToStringable(valueType))
                {
                    key.Append(value.ToString());
                }
                else
                {
                    Debug.Log("[Cache] " + valueType.Name + " cannot be ToString()'d for cache key: " + value);
                }
            }

            void AppendClass(object value, Type valueType)
            {
                var valueProperties = GenerateCacheKeyValueTypeProperties.Cache(valueType, () =>
                {
                    return valueType.GetProperties(BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Static | BindingFlags.Instance);
                });

                var valueFields = GenerateCacheKeyValueTypeFields.Cache(valueType, () =>
                {
                    return valueType.GetFields(BindingFlags.Public | BindingFlags.GetField | BindingFlags.Static | BindingFlags.Instance);
                });

                if (valueProperties?.Length > 0)
                {
                    key.Append(valueProperties.Length);

                    foreach (var pi in valueProperties)
                    {
                        if (!IsToStringable(pi.PropertyType)) continue;

                        // key.Append(pi.Name);

                        try
                        {
                            MethodInfo getMethod = pi.GetGetMethod();

                            object piValue = null;
                            if (getMethod.IsStatic)
                            {
                                piValue = pi.GetValue(null);
                            }
                            else
                            {
                                piValue = pi.GetValue(value);
                            }

                            if (piValue != null)
                                AppendString(piValue, piValue.GetType());
                        }
                        catch
                        {
                            // Swallow
                        }
                    }
                }

                if (valueFields?.Length > 0)
                {
                    key.Append(valueFields.Length);

                    foreach (var fi in valueFields)
                    {
                        if (!IsToStringable(fi.FieldType)) continue;

                        // key.Append(fi.Name);

                        try
                        {
                            object fiValue = null;
                            if (fi.IsStatic)
                            {
                                fiValue = fi.GetValue(null);
                            }
                            else
                            {
                                fiValue = fi.GetValue(value);
                            }
                            if (fiValue != null)
                                AppendString(fiValue, fiValue.GetType());
                        }
                        catch
                        {
                            // Swallow
                        }
                    }
                }
            }

            void AppendCollection(ICollection collection)
            {
                key.Append(collection.Count);

                foreach (var value in collection)
                {
                    AppendValue(value);
                }
            }

            void AppendValue(object value)
            {
                if (value == null) return;

                if (key.Length > 2000) return;

                var valueType = value.GetType();

                if (IsToStringable(valueType))
                    AppendString(value, valueType);
                else if (value is ICollection collection)
                    AppendCollection(collection);
                else if (valueType.IsClass)
                    AppendClass(value, valueType);
                else
                {
                    Debug.Log("[Cache] " + valueType.Name + " is a type that is not appendable to cacheKey (not implemented)");
                }
            }

            void AppendFieldArgument(FieldInfo field)
            {
                if (field == null) return;

                var type = field.FieldType;

                if (!IsTypeAutoCacheKeyType(type))
                {
                    return;
                }

                if (key.Length > 2000)
                {
                    key.Append(field.Name.MaxLength(4));
                    return;
                }

                try
                {
                    object value;
                    if (field.IsStatic)
                    {
                        value = field.GetValue(null);
                    }
                    else
                    {
                        value = field.GetValue(target);
                    }

                    AppendValue(value);
                }
                catch
                {
                    // Swallow
                }
            }

            var type = target.GetType();

            var fields = GenerateCacheKeyFields.Cache(type, () =>
            {
                return type.GetFields(BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public);
            });

            if (fields?.Length > 0)
            {
                if (fields.Length > 64)
                    fields = fields.Take(64).ToArray();

                foreach (var field in fields)
                {
                    AppendFieldArgument(field);
                }

                if (fields.Length == 64)
                {
                    global::Log.Error("[Cache] Variables to generate key from exceeds limit of 64: " + key.ToString().MaxLength(255));
                }
                else if (key.Length > 2000)
                {
                    global::Log.Error("[Cache] Auto generated key exceeds 2000 characters limit: " + key.ToString().MaxLength(255));
                }
            }
        }

        if (condition != null)
            key.Append(condition.Method?.Name + condition.Method?.ReturnType?.Name + "");

        var principal = Principal();

        var isAuthenticated = principal?.Identity?.IsAuthenticated == true;

        if (isAuthenticated)
            key.Append(isAuthenticated);

        if (principal is ClaimsPrincipal claimsPrincipal)
        {
            var claimsIdentity = claimsPrincipal?.Identity as ClaimsIdentity;

            if (claimsPrincipal?.Claims != null)
            {
                var roles = claimsPrincipal.Claims
                    .Where(c => c.Type == claimsIdentity.RoleClaimType ||
                           c.Type.Equals("role", StringComparison.OrdinalIgnoreCase))
                    .Select(x => x.Value);

                if (roles != null)
                    key.Append(string.Join("", roles));
            }
        }
        var raw = HttpContextInstance.Current?.Request?.QueryString.Value;
        if(raw != null && raw.Length > 0)
        {
            key.Append(raw.GetCompressedKey());
        }

        return key.ToString();
    }

    static bool SkipCache(bool skipWhenAuthenticated, bool skipWhenAdmin, Func<bool> skipWhen)
    {
        if (skipWhenAuthenticated || skipWhenAdmin || skipWhen != null)
        {
            if (skipWhenAuthenticated && IsCurrentUserAuthenticated())
                return true;

            if (skipWhenAdmin && IsCurrentUserAdmin())
                return true;

            if (skipWhen != null && skipWhen())
                return true;
        }

        return false;
    }

    static TimeSpan GetCacheDuration(CacheDuration duration)
    {
        if (duration == default)
            return TimeSpan.FromSeconds(DurationConfig);
        return TimeSpan.FromSeconds((int)duration);
    }

    static bool IsCurrentUserAuthenticated()
    {
        return Principal()?.Identity?.IsAuthenticated == true;
    }

    static bool IsCurrentUserAdmin()
    {
        return Principal()?.Identity?.IsAuthenticated == true && Principal().IsInAnyRole("Admin", "Admins", "Administrator", "Administrators", "WebAdmins", "CmsAdmins", "admin", "administrators", "administrator");
    }

    static bool IsTypeAutoCacheKeyType(Type type)
    {
        if (type.IsEnum) return true;

        if (type.Inherits(SystemType.ICollectionType)) return true;

        if (type == SystemType.UriType) return true;

        if (type.IsKeyValuePair()) return true;

        if (type.IsClass && !type.IsGenericType) return true;

        return Array.IndexOf(AutoCacheKeyStringableTypes, type) >= 0;
    }

    static bool IsToStringable(Type type)
    {
        if (type.IsEnum) return true;

        if (type.IsKeyValuePair()) return true;

        return Array.IndexOf(AutoCacheKeyStringableTypes, type) >= 0;
    }

    static Type AutoCacheKeyICollection = typeof(ICollection);

    static Type[] AutoCacheKeyStringableTypes = {
            typeof(string),
            typeof(StringBuilder),
            typeof(bool),
            typeof(int),
            typeof(DateTime),
            typeof(DateTimeOffset),
            typeof(float),
            typeof(double),
            typeof(Enum),
            typeof(short),
            typeof(long),
            typeof(decimal),
            typeof(uint),
            typeof(Uri),
            typeof(TimeSpan),
            typeof(Guid),
            typeof(KeyValuePair<,>)
    };


    // TODO: Implement this somehow, EntriesCollection do not work in .NET7 as we disable 'Stats'
    static long TotalCount
    {
        get
        {
            long sum = 0;

            var entries = typeof(MemoryCache).GetProperty("EntriesCollection", BindingFlags.NonPublic | BindingFlags.Instance);

            if (entries == null) return sum;

            for (int i = 0; i < MaxCacheContainers; i++)
            {
                if (cache[i] == null) continue;

                var entriesCollection = entries.GetValue(cache[i]) as ICollection;

                if (entriesCollection == null || entriesCollection.Count == 0) continue;

                sum += entriesCollection.Count;
            }

            return sum;
        }
    }
}
