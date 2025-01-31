using System.Text.Json;
using System.Text.Json.Serialization;

namespace SystemLibrary.Common.Framework;

internal class EnumStringConverterFactory : JsonConverterFactory
{
    static Type EnumStringConverterGenericType = typeof(EnumStringConverter<>);

    static ContinueConverterFactory ContinueConverterFactory = new ContinueConverterFactory();

    public EnumStringConverterFactory(JsonNamingPolicy namingPolicy = null, bool allowIntegerValues = true)
    {
    }

    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsEnum;
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        if (typeToConvert.IsEnum)
        {
            return (JsonConverter)Activator.CreateInstance(EnumStringConverterGenericType.MakeGenericType(typeToConvert));
        }
        return ContinueConverterFactory;
    }
}
