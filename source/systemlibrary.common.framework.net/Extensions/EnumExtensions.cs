using System;
using System.Collections.Generic;
using System.Reflection;

using SystemLibrary.Common.Framework.Attributes;

namespace SystemLibrary.Common.Framework.Extensions;

/// <summary>
/// This class contains extension methods for Enum as a Generic Type
/// </summary>
/// <remarks>
/// Current version of C# does not allow extension methods 'generic types', hence these are static methods
/// </remarks>
public static class EnumExtensions<TEnum> where TEnum : IComparable, IFormattable, IConvertible
{
    /// <summary>
    /// Get all keys in the Enum as an IEnumerable of string
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
    /// var keys = SystemLibrary.Common.Framework.Extensions.EnumExtesions&lt;Color&gt;.GetKeys();
    /// 
    /// // keys[0] is 'Black'
    /// // keys[1] is 'White
    /// // Note: it returns the Keys converted to strings only, ignoring EnumText attribute
    /// </code>
    /// </example>
    /// <returns>Keys as an IEnumerable of string</returns>
    public static IEnumerable<string> GetKeys()
    {
        var type = typeof(TEnum);
        if (!type.IsEnum) throw new Exception("Could not get values from a non-enum object. " + type.Name + " is not an Enum");

        var values = Enum.GetValues(type);

        foreach (var value in values)
        {
            yield return value.ToString();
        }
    }

    /// <summary>
    /// Get all keys as the enum value itself
    /// </summary>
    /// <example>
    /// <code class="language-csharp hljs">
    /// enum Color 
    /// {
    ///     [EnumText("Black Colored Text")]
    ///     Black,
    ///     White
    /// }
    /// var enums = SystemLibrary.Common.Framework.Extensions.EnumExtesions&lt;Color&gt;.GetEnums();
    /// 
    /// // enums is now:
    /// // enums[0] is Color.Black
    /// // enums[1] is Color.White
    /// </code>
    /// </example>
    /// <returns>IEnumerable of Enum of your choice</returns>
    public static IEnumerable<TEnum> GetEnums()
    {
        var type = typeof(TEnum);
        if (!type.IsEnum) throw new Exception("Could not get values from a non-enum object. " + type.Name + " is not an Enum");

        var values = Enum.GetValues(type);

        foreach (var value in values)
        {
            yield return (TEnum)value;
        }
    }

    public static IEnumerable<string> GetValues()
    {
        var enums = GetEnums();

        foreach (var value in enums)
        {
            var e = value as Enum;

            yield return e.ToValue();
        }
    }
}

/// <summary>
/// Extension methods for Enums like ToText(), ToValue(), IsAny(), ...
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// Check if Enum is matching any in the array
    /// </summary>
    /// <example>
    /// <code class="language-csharp hljs">
    /// enum Color {
    ///     Black,
    ///     White,
    ///     Red,
    ///     Blue
    /// }
    /// 
    /// var red = Color.Red;
    /// 
    /// if(red.IsAny(Color.Black, Color.Blue)) {
    ///     //Never hit, red is Red, never blue/black
    /// }
    /// </code>
    /// </example>
    /// <returns>True or false</returns>
    public static bool IsAny(this Enum enumField, params Enum[] values)
    {
        if (values == null) return false;

        foreach (var value in values)
            if (Equals(enumField, value))
                return true;

        return false;
    }

    /// <summary>
    /// Gets the EnumText attribute's value, fallback to enumField.ToString()
    /// </summary>
    /// <example>
    /// <code class="language-csharp hljs">
    /// enum EnumColor
    /// {
    ///     [EnumText("White")]
    ///     [EnumValue("BlackAndWhite")]
    ///     Black,
    ///     
    ///     Pink
    /// }
    ///
    /// var value = EnumColor.Black.ToText();
    /// // White
    /// 
    /// var value = EnumColor.Pink.ToText();
    /// // Pink
    /// </code>
    /// </example>
    /// <returns>Returns the text in EnumText attribute or Enum as string or null if nullwas passed</returns>
    public static string ToText(this Enum enumField)
    {
        if (enumField == null) return null;

        var textAttribute = GetAttribute<EnumTextAttribute>(enumField, SystemType.EnumTextAttributeType);

        if (textAttribute != null)
            return textAttribute.Text;

        return enumField?.ToString();
    }

    /// <summary>
    /// Gets the EnumText attribute's object value
    /// </summary>
    /// <example>
    /// <code class="language-csharp hljs">
    /// enum EnumColor
    /// {
    ///     [EnumText("White")]
    ///     [EnumValue(1234)]
    ///     Black,
    ///     
    ///     [EnumText((string)null)]
    ///     Red,
    ///     Pink
    /// }
    ///
    /// var value = EnumColor.Black.GetEnumText();
    /// // "White"
    /// 
    /// var value = EnumColor.Red.GetEnumText();
    /// // null
    /// 
    /// var value = EnumColor.Pink.GetEnumText();
    /// // null
    /// </code>
    /// </example>
    /// <returns>Returns EnumText attributes value or null if not defined</returns>
    public static string GetEnumText(this Enum enumField)
    {
        if (enumField == null) return null;

        var textAttribute = GetAttribute<EnumTextAttribute>(enumField, SystemType.EnumTextAttributeType);

        return textAttribute?.Text;
    }

    /// <summary>
    /// Gets the EnumValue attribute's value, fallback to enumField.ToString()
    /// </summary>
    /// <example>
    /// <code class="language-csharp hljs">
    /// enum EnumColor
    /// {
    ///     [EnumText("White")]
    ///     [EnumValue(1234)]
    ///     Black,
    ///     
    ///     [EnumValue(null)]
    ///     Blue,
    ///     
    ///     Pink
    /// }
    ///
    /// var value = EnumColor.Black.ToValue();
    /// // "1234" as string
    /// 
    /// var value = EnumColor.Pink.ToValue();
    /// // Pink, does not have the EnumValue attribute so it falls back to Pink as a string
    /// 
    /// var value = EnumColor.Blue.ToValue();
    /// // value is null, as Blue have EnumValue null
    /// </code>
    /// </example>
    /// <returns>Returns value of EnumValue attribute if exist, or Enum as string</returns>
    public static string ToValue(this Enum enumField)
    {
        if (enumField == null) return null;

        var valueAttribute = GetAttribute<EnumValueAttribute>(enumField, SystemType.EnumValueAttributeType);

        if (valueAttribute != null)
        {
            return valueAttribute.Value?.ToString();
        }

        var value = enumField.ToString();

        if (value != null && value.Length > 1 && value[0] == '_' && char.IsDigit(value[1]))
        {
            // NOTE: Will remove underscore when format is: _[digits][any text]
            if (value.Length > 2)
            {
                if (char.IsDigit(value[2]))
                {
                    return value.Substring(1);
                }
            }
            else
            {
                return value.Substring(1);
            }
        }
        return value;
    }

    /// <summary>
    /// Gets the EnumValue-attribute's object value
    /// </summary>
    /// <example>
    /// <code class="language-csharp hljs">
    /// enum EnumColor
    /// {
    ///     [EnumText("White")]
    ///     [EnumValue(1234)]
    ///     Black,
    ///     
    ///     [EnumValue(null)]
    ///     Blue,
    ///     
    ///     Pink
    /// }
    ///
    /// var value = EnumColor.Black.GetEnumValue();
    /// // 1234, an int
    /// 
    /// var value = EnumColor.Pink.GetEnumValue();
    /// // null as Pink does not have the EnumValue attribute
    /// 
    /// var value = EnumColor.Blue.GetEnumValue();
    /// // null as Blue have null as the EnumValue
    /// </code>
    /// </example>
    /// <returns>Returns EnumValue attribute's value or null if not exist</returns>
    public static object GetEnumValue(this Enum enumField)
    {
        if (enumField == null) return null;

        var valueAttribute = GetAttribute<EnumValueAttribute>(enumField, SystemType.EnumValueAttributeType);

        return valueAttribute?.Value;
    }

    static T GetAttribute<T>(object value, Type type) where T : Attribute
    {
        if (value == null) return default;

        var enumType = value.GetType();

        var members = enumType.GetMember(value.ToString());

        return GetFirstAttributeFromMembers<T>(members, type);
    }

    static T GetFirstAttributeFromMembers<T>(MemberInfo[] members, Type type) where T : Attribute
    {
        if (members?.Length > 0)
        {
            foreach (var member in members)
            {
                var attributes = member.GetCustomAttributes(type, inherit: false);
                if (attributes.IsNot()) continue;

                return (T)attributes[0];
            }
        }
        return default;
    }
}
