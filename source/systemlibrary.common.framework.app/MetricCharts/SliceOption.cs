namespace SystemLibrary.Common.Framework;

public class SliceOption
{
    /// <summary>
    /// Use the same segment text that you used when invoking Metric.Inc() to color and order it, or set segment to null to set a default color in the pie chart
    /// </summary>
    public string Segment;

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
