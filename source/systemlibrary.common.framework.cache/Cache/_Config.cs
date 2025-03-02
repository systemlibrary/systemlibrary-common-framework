namespace SystemLibrary.Common.Framework.App;

partial class Cache
{
    static int? _DurationConfig;
    static int DurationConfig
    {
        get
        {
            if (_DurationConfig == null)
                _DurationConfig = FrameworkConfigInstance.Current.Cache.Duration;

            return _DurationConfig.Value;
        }
    }

    static int? _FallbackDurationConfig;
    static int FallbackDurationConfig
    {
        get
        {
            if (_FallbackDurationConfig == null)
            {
                _FallbackDurationConfig = FrameworkConfigInstance.Current.Cache.FallbackDuration;
            }

            return _FallbackDurationConfig.Value;
        }
    }

    static int? _ContainerSizeLimitConfig;
    static int ContainerSizeLimitConfig
    {
        get
        {
            if (_ContainerSizeLimitConfig == null)
                _ContainerSizeLimitConfig = FrameworkConfigInstance.Current.Cache.ContainerSizeLimit;

            return _ContainerSizeLimitConfig.Value;
        }
    }

    static bool? _EnablePrometheusConfig;
    static bool EnablePrometheusConfig
    {
        get
        {
            if (_EnablePrometheusConfig == null)
                _EnablePrometheusConfig = FrameworkConfigInstance.Current.Metrics.EnablePrometheus;

            return _EnablePrometheusConfig.Value;
        }
    }
}
