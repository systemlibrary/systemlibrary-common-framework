using System.Collections.Concurrent;

using Prometheus;

namespace SystemLibrary.Common.Framework;

public static class Metric
{
    internal static ConcurrentDictionary<string, MetricOption> MetricOptions = new();

    static Counter LabelCounter = Metrics.CreateCounter("total_label", "Total per label",
    new CounterConfiguration { LabelNames = new[] { "label" } });

    static Counter LabelCategoryCounter = Metrics.CreateCounter("total_label_category", "Total per label and category",
        new CounterConfiguration { LabelNames = new[] { "label", "category" } });

    static Counter LabelCategoryStatusCounter = Metrics.CreateCounter("total_label_category_status", "Total per label, category, and status",
    new CounterConfiguration { LabelNames = new[] { "label", "category", "status" } });

    public static void Init(MetricOption option = null)
    {
        if (option?.DisplayLabel == null) return;

        var key = option.DisplayLabel;

        if (MetricOptions.ContainsKey(key))
            MetricOptions.Remove(key, out _);

        MetricOptions.TryAdd(key, option);
    }

    public static void Inc(string label, string category = null, string status = null)
    {
        if (category == null)
        {
            LabelCounter.WithLabels(label).Inc();
        }
        else if (status == null)
        {
            LabelCategoryCounter.WithLabels(label, category).Inc();
        }
        else
        {
            LabelCategoryStatusCounter.WithLabels(label, category, status).Inc();
        }
    }
}
