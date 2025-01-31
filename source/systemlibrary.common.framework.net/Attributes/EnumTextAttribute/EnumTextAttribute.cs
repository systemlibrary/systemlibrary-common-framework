namespace SystemLibrary.Common.Framework;

/// <summary>
/// Decorate Enum Key with text
/// </summary>
/// <example>
/// <code class="language-csharp hljs">
/// enum Color 
/// {
///     [EnumText("Black Colored Text")]
///     Black,
///     White
/// }
/// 
/// var color = Color.Black;
/// 
/// var value = color.ToText();
/// 
/// var value2 = Color.White.ToText();
/// 
/// // 'value' is now "Black Colored Text"
/// // 'value2' is now "White", 
/// // Note: .ToText() falls back to ToString() of the enum key
/// 
/// var value = Color.White.GetEnumText();
/// // 'value' is now null, as White does not contain EnumTextAttribute
/// 
/// var value = Color.Black.GetEnumText();
/// // 'value' is now a string with value 'Black Colored Text' as 'Black' has the EnumTextAttribute
/// 
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class EnumTextAttribute : Attribute
{
    public string Text;

    /// <param name="text">Set additional text metadata for the Enum key</param>
    public EnumTextAttribute(string text = null)
    {
        Text = text;
    }
}