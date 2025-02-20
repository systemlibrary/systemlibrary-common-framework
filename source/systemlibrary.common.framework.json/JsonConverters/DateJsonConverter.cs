using System.Text.Json;
using System.Text.Json.Serialization;

namespace SystemLibrary.Common.Framework;

/// <summary>
/// Date json converter converts a DateTime to 'yyyy-MM-dd'
/// <para>Useful when you just need the date, ignoring time, by adding this to the datetime property through [JsonConverter] attribute</para>
/// </summary>
public class DateJsonConverter : JsonConverter<DateTime>
{
    public DateJsonConverter()
    {
    }

    public override void Write(Utf8JsonWriter writer, DateTime date, JsonSerializerOptions options)
    {
        writer.WriteStringValue(date.ToString("yyyy-MM-dd"));
    }

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.GetString().ToDateTime("yyyy-MM-dd");
    }
}
