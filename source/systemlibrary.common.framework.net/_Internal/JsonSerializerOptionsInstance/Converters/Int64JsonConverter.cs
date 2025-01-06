using System.Text.Json;
using System.Text.Json.Serialization;

namespace SystemLibrary.Common.Framework;

internal class LongJsonConverter : JsonConverter<long>
{
    public override long Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return 0;

        if (reader.TokenType == JsonTokenType.Number)
        {
            if (reader.TryGetInt64(out long value)) return value;

            return Convert.ToInt64(reader.GetDouble());
        }

        if (reader.TokenType == JsonTokenType.True)
            return 1;

        if (reader.TokenType == JsonTokenType.False)
            return 0;

        var v = reader.GetString();

        if (long.TryParse(v, out long f))
        {
            return f;
        }
        else if (double.TryParse(v, out double d))
        {
            return Convert.ToInt64(d);
        }
        else
        {
            throw new JsonException("Error reading: " + reader.GetString() + " into an Int64/long");
        }
    }

    public override void Write(Utf8JsonWriter writer, long value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value);
    }
}
