using System.Collections.Concurrent;

using Prometheus;

namespace SystemLibrary.Common.Framework;

/// <summary>
/// Create a metric option and pass it into the Metric.Init method to control how the pie chart of a metric is rendered
/// <para>Note: A pie chart's key is made up of label + category + status, so if you specify the exact same label, category and status, it overrides the existing one</para>
/// </summary>
public class MetricOption
{
    /// <summary>
    /// Label of the pie chart, also used as the key to hold the option object for this particular pie chart
    /// <para>Registering a MetricOption with the same Label will override any existing registration</para>
    /// </summary>
    public string Label;

    /// <summary>
    /// True to show animation upon loading pie chart metric UI
    /// </summary>
    public bool ShowAnimation = true;

    /// <summary>
    /// True to show the legend toolbar menu above the pie chart
    /// </summary>
    public bool ShowLegend = false;

    /// <summary>
    /// Set the default text color of the pie chart
    /// </summary>
    public string TextColor = "#fff";

    /// <summary>
    /// Each slice in the pie chart with some styling options
    /// </summary>
    public SliceOption[] Slices;
}

internal class MetricOptionResponse
{
    public string label;
    public bool showAnimation;
    public bool showLegend;
    public string textColor;
}

public class SliceOption
{
    /// <summary>
    /// Optional: if you call Metric.Inc with a category, specify the same category here to order and color it
    /// </summary>
    public string Category;

    /// <summary>
    /// Optional: if you call Metric.Inc with a status, specify the same status here to order and color it
    /// </summary>
    public string Status;

    /// <summary>
    /// Background color for the slice, pass either rgb, hex or a default predefined color name
    /// <para>Example 1: rgb(255, 100, 100)</para>
    /// <para>Example 2: #FF6464</para>
    /// <para>Example 3: red</para>
    /// </summary>
    public string Color = null;

    /// <summary>
    /// Order of the slice in the pie chart
    /// <para>Note: order 0 is first, then it increases and renders slices clockwise. An order of 9999999 will use a sort order based on Count, highest first</para>
    /// </summary>
    public int Order = 9999999;
}

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
        if (option?.Label == null) return;

        var key = option.Label;

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
