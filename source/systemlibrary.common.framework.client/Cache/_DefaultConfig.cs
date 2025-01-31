namespace SystemLibrary.Common.Framework.App;

partial class Cache
{
    internal const int DefaultDuration = 180;
    internal const int DefaultFallbackDuration = 300;
    internal const int DefaultContainerSizeLimit = 60000;
    internal const bool DefaultEnablePrometheus = true;

    static int? _DurationConfig;
    static int DurationConfig
    {
        get
        {
            if (_DurationConfig == null)
            {
                _DurationConfig = AppSettings.Current.SystemLibraryCommonFramework.Cache.Duration;

                if (_DurationConfig == null || _DurationConfig <= 0)
                {
                    _DurationConfig = DefaultDuration;
                }
            }

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
                _FallbackDurationConfig = AppSettings.Current.SystemLibraryCommonFramework.Cache.FallbackDuration;

                if (_FallbackDurationConfig == null || _FallbackDurationConfig < 0)
                {
                    _FallbackDurationConfig = DefaultFallbackDuration;
                }
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
            {
                _ContainerSizeLimitConfig = AppSettings.Current.SystemLibraryCommonFramework.Cache.ContainerSizeLimit;

                if (_ContainerSizeLimitConfig == null || _ContainerSizeLimitConfig < 10)
                {
                    _ContainerSizeLimitConfig = DefaultContainerSizeLimit;
                }
            }

            return _ContainerSizeLimitConfig.Value;
        }
    }

    static bool? _EnablePrometheusConfig;
    static bool EnablePrometheusConfig
    {
        get
        {
            if (_EnablePrometheusConfig == null)
            {
                _EnablePrometheusConfig = AppSettings.Current?.SystemLibraryCommonFramework?.Metrics?.EnablePrometheus;

                if (_EnablePrometheusConfig == null)
                {
                    _EnablePrometheusConfig = DefaultEnablePrometheus;
                }
            }

            return _EnablePrometheusConfig.Value;
        }
    }
}
