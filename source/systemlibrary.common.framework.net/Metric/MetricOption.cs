namespace SystemLibrary.Common.Framework;

/// <summary>
/// Create a metric option and pass it into the Metric.Init method to control how the pie chart of a metric is rendered
/// <para>Note: A pie chart's key is made up of label + category + status, so if you specify the exact same label, category and status, it overrides the existing one</para>
/// </summary>
public class MetricOption
{
    /// <summary>
    /// Set pie chart options for the specific display label, usually it is only the label, but if you use both category and status, the display label is made up of label and category, delimited by :
    /// <para>Registering a new metric option with the same DisplayLabel will override previous registration</para>
    /// </summary>
    public string DisplayLabel;

    /// <summary>
    /// True to show animation upon loading pie chart metric UI
    /// </summary>
    public bool ShowAnimation = true;

    /// <summary>
    /// True to show the legend toolbar menu above the pie chart, which will hide the 'DisplayLabel' for the pie chart
    /// <para>Need to know the DisplayLabel for a pie chart? Simply set ShowLegend to false and recompile and check</para>
    /// </summary>
    public bool ShowLegend = false;

    /// <summary>
    /// True to show a black border between each slice
    /// </summary>
    public bool ShowBorder = false;

    /// <summary>
    /// Set the default text color of the pie chart
    /// <para>Supports hex and rgb(0,0,0)</para>
    /// </summary>
    public string TextColor = "#fff";

    /// <summary>
    /// Each slice in the pie chart with some styling options
    /// </summary>
    public SliceOption[] Slices;
}