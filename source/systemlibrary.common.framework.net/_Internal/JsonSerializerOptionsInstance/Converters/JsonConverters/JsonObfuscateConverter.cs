using System.Text.Json;

using SystemLibrary.Common.Framework.Attributes;
using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework;

internal class ObfuscateJsonConverter : BaseJsonConverter
{
    JsonObfuscateAttribute Attribute;

    public ObfuscateJsonConverter(JsonObfuscateAttribute attribute)
    {
        Attribute = attribute;
    }

    public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = GetValue(ref reader, typeToConvert);

        if (value is not byte[] b) return value;

        if (b != null && b.Length > 0)
        {
            var base64 = b.ToBase64();

            var devalued = base64.FromBase64().Deobfuscate(Attribute.Salt);
        
            return GetDeValued(devalued, typeToConvert);
        }

        return typeToConvert.Default();
    }

    public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
    {
        var data = value?.ToString();

        if (data.IsNot()) return;

        var encrypted = data.Obfuscate(Attribute.Salt).ToBase64();

        writer.WriteStringValue(encrypted);
    }
}
