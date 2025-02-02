using System.ComponentModel;

namespace SystemLibrary.Common.Framework;

internal static class ConfigEnumConvertRegister
{
    internal static void Register()
    {
        var enumType = typeof(Enum);
        var enumTypeConverter = TypeDescriptor.GetConverter(enumType);
        if (enumTypeConverter == null || !(enumTypeConverter is ConfigEnumConverter))
        {
            TypeDescriptor.AddAttributes(enumType, new TypeConverterAttribute(typeof(ConfigEnumConverter)));
        }
    }
}
