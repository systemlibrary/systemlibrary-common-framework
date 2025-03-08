using System.Text.Json;
using System.Text.Json.Serialization;

namespace SystemLibrary.Common.Framework;

/// <summary>
/// DateTime json converter that takes a string format
/// </summary>
/// <remarks>
/// Used internally
/// Exposed if someone needs a different format that this Library does not support out of the box when using Json() extension method
/// </remarks>
/// <example>
/// Example of an additional datetime converter:
/// <code>
/// var options = new JsonSerializationOptions();
/// options.Converters.Add(new DateTimeJsonConverter("yyyy/MM/dd hh:mm"));
/// </code>
/// </example>
public class DateTimeOffsetJsonConverter : JsonConverter<DateTimeOffset>
{
    string Format;

    public DateTimeOffsetJsonConverter(string format)
    {
        Format = format;
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset date, JsonSerializerOptions options)
    {
        writer.WriteStringValue(date.ToString(Format));
    }

    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.GetString().ToDateTimeOffset(Format);
    }
}
