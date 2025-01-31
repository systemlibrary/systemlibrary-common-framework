using System.ComponentModel;
using System.Globalization;
namespace SystemLibrary.Common.Framework.App;

internal class ExtendedEnumConverter : EnumConverter
{
    public ExtendedEnumConverter(Type type) : base(type) { }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value is string str)
            return str.ToEnum(EnumType);

        return base.ConvertFrom(context, culture, value);
    }
}