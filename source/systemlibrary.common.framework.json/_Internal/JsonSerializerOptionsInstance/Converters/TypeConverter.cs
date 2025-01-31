using System.Text.Json;
using System.Text.Json.Serialization;

namespace SystemLibrary.Common.Framework;

// Creds: https://stackoverflow.com/questions/66919668/net-core-graphql-graphql-systemtextjson-serialization-and-deserialization-of
internal class TypeConverter : JsonConverter<Type>
{
    public override Type Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
        )
    {
        var assemblyQualifiedName = reader.GetString();

        if (assemblyQualifiedName == null) return default;

        return Type.GetType(assemblyQualifiedName);
    }

    public override void Write(
        Utf8JsonWriter writer,
        Type value,
        JsonSerializerOptions options
        )
    {
        writer.WriteStringValue(value?.AssemblyQualifiedName);
    }
}
