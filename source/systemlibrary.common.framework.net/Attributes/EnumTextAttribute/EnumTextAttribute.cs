namespace SystemLibrary.Common.Framework;

/// <summary>
/// Decorate the enum key with text.
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
/// // Note: .ToText() falls back to the ToString() representation of the enum key.
/// 
/// var value = Color.White.GetEnumText();
/// // 'value' is now null, as 'White' does not contain the EnumTextAttribute.
/// 
/// var value = Color.Black.GetEnumText();
/// // 'value' is now a string with the value 'Black Colored Text', as 'Black' has the EnumTextAttribute.
/// 
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class EnumTextAttribute : Attribute
{
    public string Text;

    /// <param name="text">Sets additional text metadata for the enum key.</param>
    public EnumTextAttribute(string text = null)
    {
        Text = text;
    }
}