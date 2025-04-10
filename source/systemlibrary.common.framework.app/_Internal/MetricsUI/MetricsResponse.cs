namespace SystemLibrary.Common.Framework;

internal class MetricsResponse
{
    public List<MetricData> Metrics { get; set; } = new List<MetricData>();

    public List<MetricOptionResponse> Options { get; set; }
}