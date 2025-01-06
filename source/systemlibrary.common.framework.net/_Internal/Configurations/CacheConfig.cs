namespace SystemLibrary.Common.Framework;

internal class CacheConfig
{
    public int Duration { get; set; } = 180; // 3 minutes
    public int ContainerSizeLimit { get; set; } = 60000;
    public int FallbackDuration { get; set; } = 300; // 5 minutes
}
