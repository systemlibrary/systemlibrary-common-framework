using System.ComponentModel;
using System.Globalization;
namespace SystemLibrary.Common.Framework;

internal class ConfigEnumConverter : EnumConverter
{
    public ConfigEnumConverter(Type type) : base(type) { }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value is string str)
            return str.ToEnum(EnumType);

        return base.ConvertFrom(context, culture, value);
    }
}