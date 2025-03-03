using Prometheus;

internal static class CacheMetrics
{
    static Counter CacheMissWithFallbackCounter = Metrics.CreateCounter("cache_miss_with_fallback_total", "Total number of cache misses where a fallback value was found.");
    static Counter CacheExceptionWithFallbackCounter = Metrics.CreateCounter("cache_exception_with_fallback_total", "Total number of cache exceptions where a fallback value was found.");
    static Counter CacheLookupExceptionsCounter = Metrics.CreateCounter("cache_lookup_exceptions_total", "Total number of cache lookup exceptions.");

    internal static void RecordCacheMissWithFallbackCounter() => CacheMissWithFallbackCounter.Inc();
    internal static void RecordCacheExceptionWithFallbackCounter() => CacheExceptionWithFallbackCounter.Inc();
    internal static void RecordCacheLookupExceptionsCounter() => CacheLookupExceptionsCounter.Inc();

    static Counter CacheHitCounter = Metrics.CreateCounter("cache_hits_total", "Total cache hits.");
    static Counter CacheMissCounter = Metrics.CreateCounter("cache_miss_total", "Total cache miss.");
    static Counter CacheIgnoredCounter = Metrics.CreateCounter("cache_ignored_total", "Total cache ignored.");

    internal static void RecordCacheHit() => CacheHitCounter.Inc();
    internal static void RecordCacheMiss() => CacheMissCounter.Inc();
    internal static void RecordCacheIgnored() => CacheIgnoredCounter.Inc();
}

