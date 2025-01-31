using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SystemLibrary.Common.Framework;

/// <summary>
/// Use to convert a string to DateTime during Json() invocation with your own format
/// </summary>
/// <remarks>
/// Used internally
/// Exposed if someone needs a different format that this Library does not support out of the box when using Json() extension method
/// </remarks>
/// <example>
/// Example:
/// <code>
/// var options = new JsonSerializationOptions();
/// options.Converters.Add(new DateTimeJsonConverter("yyyy/MM/dd hh:mm"));
/// 
/// // Assume "json" is a string and somewhere there's a date on format "2000/12/24 12:30"
/// var result = json.Json(options);
/// </code>
/// </example>
public class DateTimeJsonConverter : JsonConverter<DateTime>
{
    string Format;

    public DateTimeJsonConverter(string format = null)
    {
        Format = format;
    }

    public override void Write(Utf8JsonWriter writer, DateTime date, JsonSerializerOptions options)
    {
        writer.WriteStringValue(date.ToString(Format));
    }

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.GetString().ToDateTime(Format);
    }
}

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
