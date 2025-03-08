namespace SystemLibrary.Common.Framework.App;

partial class Cache
{
    static int? _DurationConfig;
    static int DurationConfig => _DurationConfig ??= FrameworkConfigInstance.Current.Cache.Duration;

    static int? _FallbackDurationConfig;
    static int FallbackDurationConfig => _FallbackDurationConfig ??= FrameworkConfigInstance.Current.Cache.FallbackDuration;

    static int? _ContainerSizeLimitConfig;
    static int ContainerSizeLimitConfig => _ContainerSizeLimitConfig ??= FrameworkConfigInstance.Current.Cache.ContainerSizeLimit;


    static bool? _EnablePrometheusConfig;
    static bool EnablePrometheusConfig => _EnablePrometheusConfig ??= FrameworkConfigInstance.Current.Metrics.EnablePrometheus;

}
