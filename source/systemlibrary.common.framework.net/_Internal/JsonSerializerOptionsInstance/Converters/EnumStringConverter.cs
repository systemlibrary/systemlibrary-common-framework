using System.Text.Json;
using System.Text.Json.Serialization;

using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework;

internal class EnumStringConverter<TEnum> : JsonConverter<TEnum> where TEnum : IComparable, IFormattable, IConvertible
{
    Type Type;

    public EnumStringConverter()
    {
        Type = typeof(TEnum);
    }

    public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            if (reader.TryGetInt32(out int i32))
                return (TEnum)i32.ToString().ToEnum(Type);

            if (reader.TryGetInt64(out long i64))
                return (TEnum)(object)i64;
        }

        if (reader.TokenType == JsonTokenType.Null || reader.TokenType == JsonTokenType.False)
            return default;

        if (reader.TokenType == JsonTokenType.String)
        {
            return (TEnum)reader.GetString().ToEnum(Type);
        }
        throw new JsonException("Could not convert token-type " + reader.TokenType + " to Enum " + Type.Name);
    }

    public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
    {
        if (value == null)
            writer.WriteNumberValue(0);
        else
        {
            writer.WriteStringValue(((Enum)(object)value).ToValue());
        }
    }
}
