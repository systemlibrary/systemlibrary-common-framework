
namespace SystemLibrary.Common.Framework.App;

public enum CacheDuration
{
    /// <summary>
    /// An 'unset' cache duration or 0, will use the default cache duration configured in appSettings
    /// </summary>
    Unset = 0,

    /// <summary>
    /// 20 seconds for real-time, low-latency needs
    /// </summary>
    XS = 20,

    /// <summary>
    /// 3 minutes for frequent refreshes
    /// </summary>
    S = 180,

    /// <summary>
    /// 20 minutes for moderate-refresh data
    /// </summary>
    M = 1200,

    /// <summary>
    /// 2 hours for data rarely changing or large and expensive data
    /// </summary>
    L = 7200,

    /// <summary>
    /// 1 day for data that changes, theres no harm in just showing it the next day
    /// </summary>
    XL = 86400,

    /// <summary>
    /// 2 weeks for static data
    /// </summary>
    XXL = 1209600
}