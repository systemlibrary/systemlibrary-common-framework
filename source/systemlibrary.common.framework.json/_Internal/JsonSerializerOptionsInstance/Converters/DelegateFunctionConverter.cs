using System.Text.Json;
using System.Text.Json.Serialization;

namespace SystemLibrary.Common.Framework;

public class DelegateJsonConverter : JsonConverter<object>
{
    public override bool CanConvert(Type typeToConvert)
    {
        return SystemType.DelegateType.IsAssignableFrom(typeToConvert);
    }

    public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        reader.Skip();
        return null;
    }

    public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
    {
    }
}

