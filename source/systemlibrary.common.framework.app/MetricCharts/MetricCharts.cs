using System.Collections.Concurrent;

namespace SystemLibrary.Common.Framework;

public static class MetricCharts
{
    internal static ConcurrentDictionary<string, MetricChartOption> MetricOptions = new();

    public static void Add(MetricChartOption option = null)
    {
        if (option?.MetricLabel == null) return;

        var key = option.MetricLabel;

        if (MetricOptions.ContainsKey(key))
            MetricOptions.Remove(key, out _);

        MetricOptions.TryAdd(key, option);
    }
}