namespace SystemLibrary.Common.Framework.Attributes;

/// <summary>
/// Decorate Enum Key with a value
/// </summary>
/// <remarks>
/// The string extension Json() will try matching a value in a json-string to the EnumValue specified first, and fall back to normal behavior if no match
/// <para>The string extension Json() will output the EnumValue if specified, when converting a class to json-string</para>
/// </remarks>
/// <example>
/// <code class="language-csharp hljs">
/// enum Color 
/// {
///     [EnumValue("#000")]
///     [EnumText("Hello Black")]
///     Black,
///     
///     [EnumText("Hello White")]
///     White,
///     
///     Pink
/// }
/// 
/// var black = Color.Black;
/// var value = black.ToValue();
/// // 'value' is now "#000"
/// 
/// var value = Color.White.ToValue();
/// // 'value' is now "White"
/// 
/// var value = Color.Pink.ToValue();
/// // 'value' is now 'Pink'
/// 
/// // Note: ToValue() falls back to "ToString()" on the enum key
/// 
/// var value = Color.Pink.GetEnumValue();
/// // 'value' is now null, as Pink does not have a EnumValueAttribute
/// 
/// var value = Color.Black.GetEnumValue();
/// // 'value' is now an object with value "#000", so it is castable to string
/// // Note: GetEnumValue() returns null if the enum key does not have the attribute declared, or if the actual "value" of the EnumValueAttribute is null
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class EnumValueAttribute : Attribute
{
    public object Value;

    public EnumValueAttribute(object value)
    {
        this.Value = value;
    }
}