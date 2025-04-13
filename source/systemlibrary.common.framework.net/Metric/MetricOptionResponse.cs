using System.Collections.Concurrent;

using Prometheus;

namespace SystemLibrary.Common.Framework;

internal class MetricOptionResponse
{
    public string label;
    public bool showAnimation;
    public bool showLegend;
    public bool showBorder;
    public string textColor;
}
