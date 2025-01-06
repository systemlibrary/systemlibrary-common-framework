using System.Text.Json;

using SystemLibrary.Common.Framework.Attributes;
using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework;

internal class JsonEncryptConverter : BaseJsonConverter
{
    JsonEncryptAttribute Attribute;

    public JsonEncryptConverter(JsonEncryptAttribute attribute)
    {
        Attribute = attribute;
    }

    public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = GetValue(ref reader, typeToConvert);

        if (value is not byte[] b) return value;

        var base64 = b.ToBase64();

        var devalued = base64.Decrypt(Attribute.Key, Attribute.IV, Attribute.AddedIV);

        return GetDeValued(devalued, typeToConvert);
    }

    public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
    {
        var data = value?.ToString();

        if (data.IsNot()) return;

        var encrypted = data.Encrypt(Attribute.Key, Attribute.IV, Attribute.AddedIV);

        writer.WriteStringValue(encrypted);
    }
}
