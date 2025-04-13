namespace SystemLibrary.Common.Framework;

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
