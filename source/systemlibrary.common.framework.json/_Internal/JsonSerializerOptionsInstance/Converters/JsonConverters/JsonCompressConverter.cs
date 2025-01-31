using System.Text.Json;

using SystemLibrary.Common.Framework.Attributes;
using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework;

internal class JsonCompressConverter : BaseJsonConverter
{
    JsonCompressAttribute Attribute;

    public JsonCompressConverter(JsonCompressAttribute attribute)
    {
        Attribute = attribute;
    }

    public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value =  reader.GetBytesFromBase64(); //GetValue(ref reader, typeToConvert);

        if (value == null) return typeToConvert.Default();

        return GetDeValued(value.Decompress(), typeToConvert);
    }

    public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
    {
        var data = value?.ToString();

        if (data.IsNot()) return;

        writer.WriteStringValue(data.Compress());
    }
}
