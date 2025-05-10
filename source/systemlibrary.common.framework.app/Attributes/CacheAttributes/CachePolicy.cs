namespace SystemLibrary.Common.Framework.App;

/// <summary>
/// Add any OutputCache policies to the OutputCache attribute
/// <para>Remember to use the .ToValue() method on the Enum Key, example [OutputCache(CachePolicy.CacheRoles.ToValue())]</para>
/// </summary>
public enum CachePolicy
{
    /// <summary>
    /// Cache into two categories, one for authenticated and one for unauthenticated users
    /// </summary>
    [EnumValue("slcf__CacheAuthenticatedPolicy")]
    CacheAuthenticated,

    /// <summary>
    /// Cache based on all roles the current user have, concatinated as a one cache key string
    /// </summary>
    [EnumValue("slcf__CacheRolesPolicy")]
    CacheRoles,

    /// <summary>
    /// Cache based on common claims that most often exists (sub, phone, email, id) on the user identity
    /// </summary>
    [EnumValue("slcf__CacheUserPolicy")]
    CacheUser,

    /// <summary>
    /// Use the standard 'OutputCache' with optional tags to skip caching for admins or authenticated users:
    /// <para>skipWhenAuthenticated=true</para>
    /// <para>skipWhenAdmin=true</para>
    /// NOTE: This 'Cache Policy' is a placeholder for documentation purposes and specifies additional tags supported by this framework for the sealed class 'OutputCacheAttribute' from Microsoft.
    /// </summary>
    /// <remarks>
    /// Requires framework option UseControllers or UseMvc to be true
    /// </remarks>
    OutputCacheTags,
}
