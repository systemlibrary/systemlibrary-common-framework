using Prometheus;

namespace SystemLibrary.Common.Framework;

public static class Metric
{
    static Counter LabelCounter = Metrics.CreateCounter("total_label", "Total per label",
    new CounterConfiguration { LabelNames = new[] { "label" } });

    static Counter LabelSegmentCounter = Metrics.CreateCounter("total_label_segment", "Total per label and segment",
        new CounterConfiguration { LabelNames = new[] { "label", "segment" } });

    public static void Inc(string label, string segment = null)
    {
        if (segment == null)
        {
            LabelCounter.WithLabels(label).Inc();
        }
        else
        {
            LabelSegmentCounter.WithLabels(label, segment).Inc();
        }
    }
}
