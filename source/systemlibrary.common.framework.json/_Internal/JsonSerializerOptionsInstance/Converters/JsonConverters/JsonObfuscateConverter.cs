using System.Text.Json;

using SystemLibrary.Common.Framework.Attributes;

namespace SystemLibrary.Common.Framework;

internal class JsonObfuscateConverter : BaseJsonConverter
{
    JsonObfuscateAttribute Attribute;

    public JsonObfuscateConverter(JsonObfuscateAttribute attribute)
    {
        Attribute = attribute;
    }

    public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var txt = reader.GetString();

        return GetDeValued(txt.Deobfuscate(), typeToConvert);
    }

    public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
    {
        if (value == null) return;

        var data = value.ToString();

        writer.WriteStringValue(data.Obfuscate());
    }
}
