namespace SystemLibrary.Common.Framework;

internal class CacheConfig
{
    public int Duration { get; set; } = 200; // 3 minutes 20 seconds
    public int ContainerSizeLimit { get; set; } = 60000;
    public int FallbackDuration { get; set; } = 300; // 5 minutes
}
