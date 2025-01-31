using Prometheus;

internal static class CacheMetrics
{
   static Counter CacheMissFallbackCounter = Metrics.CreateCounter("cache_miss_with_fallback_total", "Cache miss, but fallback was found.");
   static Counter CacheExceptionFallbackCounter = Metrics.CreateCounter("cache_exception_with_fallback_total", "Cache threw exception, but fallback was found.");
   static Counter CacheExceptionNoFallbackCounter = Metrics.CreateCounter("cache_exception_no_fallback_total", "Cache threw exception and no fallback was found.");

   internal static void RecordCacheMissWithFallback() => CacheMissFallbackCounter.Inc();
   internal static void RecordCacheExceptionWithFallback() => CacheExceptionFallbackCounter.Inc();
   internal static void RecordCacheExceptionNoFallback() => CacheExceptionNoFallbackCounter.Inc();
}  

