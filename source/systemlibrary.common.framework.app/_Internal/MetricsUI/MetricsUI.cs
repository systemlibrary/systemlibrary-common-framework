using System.Text.RegularExpressions;

using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework;

internal static class MetricsUI
{
    static string IndexTemplate;

    const int DefaultLastOrder = 9999999;

    static MetricsUI()
    {
        IndexTemplate = Assemblies.GetEmbeddedResource("_Internal/MetricsUI/Index.html");
    }

    internal static string GetHtmlView(string metricsData)
    {
        var data = ParseMetricData(metricsData);

        return IndexTemplate.Replace("//@data@", data.Json());
    }

    internal static MetricsResponse ParseMetricData(string metricsData)
    {
        var response = new MetricsResponse();

        if (metricsData.IsNot()) return response;

        using var reader = new StringReader(metricsData);
        string line;

        while ((line = reader.ReadLine()) != null)
        {
            if (line.StartsWith("#")) continue;

            var parts = line.Split(new[] { ' ' }, 2);
            if (parts.Length != 2 || !double.TryParse(parts[1], out double value)) continue;

            var labels = ExtractLabels(parts[0]);
            var existingMetric = FindExistingMetric(response, labels.label, labels.category, labels.status);

            if (existingMetric != null)
            {
                existingMetric.Count += (int)value;
            }
            else
            {
                response.Metrics.Add(new MetricData
                {
                    Label = labels.label,
                    Category = labels.category,
                    Status = labels.status,
                    Count = (int)value,
                    Order = GetMetricSliceOrder(labels.label, labels.category, labels.status),
                    Color = GetMetricSliceColor(labels.label, labels.category, labels.status)
                });
            }
        }

        if (Metric.MetricOptions.Count > 0 && response.Metrics.Count > 0)
        {
            response.Options = new List<MetricOptionResponse>();

            foreach (var option in Metric.MetricOptions)
            {
                response.Options.Add(new MetricOptionResponse
                {
                    label = option.Value.DisplayLabel,
                    showAnimation = option.Value.ShowAnimation,
                    showLegend = option.Value.ShowLegend,
                    showBorder = option.Value.ShowBorder,
                    textColor = option.Value.TextColor,
                });
            }
        }
        return response;
    }

    static int GetMetricSliceOrder(string label, string category, string status)
    {
        var key = label;

        if (!Metric.MetricOptions.ContainsKey(key)) return DefaultLastOrder;

        var option = Metric.MetricOptions[label];

        var slice = option?.Slices?.FirstOrDefault(s => s.Category == category && s.Status == status);

        return slice?.Order ?? DefaultLastOrder;
    }

    static string GetMetricSliceColor(string label, string category, string status)
    {
        var key = label;

        if (!Metric.MetricOptions.ContainsKey(key)) return null;

        var option = Metric.MetricOptions[label];

        var slice = option?.Slices?.FirstOrDefault(s => s.Category == category && s.Status == status);

        return slice?.Color;
    }

    static MetricData FindExistingMetric(MetricsResponse response, string label, string category, string status) =>
        response.Metrics.FirstOrDefault(m =>
            m.Label == label &&
            (m.Category == category || category == null) &&
            (m.Status == status || status == null));

    static (string label, string category, string status) ExtractLabels(string metricName)
    {
        var regex = new Regex(@"(\w+)=""([^""]+)""");
        var matches = regex.Matches(metricName);

        string label = "", category = null, status = null;

        foreach (Match match in matches)
        {
            var key = match.Groups[1].Value;
            var value = match.Groups[2].Value;

            if (key == "label") label = value;
            else if (key == "category") category = value;
            else if (key == "status") status = value;
        }

        return (label, category, status);
    }
}
