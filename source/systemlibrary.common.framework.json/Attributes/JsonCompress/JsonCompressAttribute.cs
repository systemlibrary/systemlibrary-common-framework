using System.Text.Json.Serialization;

namespace SystemLibrary.Common.Framework.Attributes;

/// <summary>
/// Compress and decompress a String, Long or Int property or field on serialization and deserialization
/// <para>Useful when you want to minify long texts before sending to Client or upon receiving</para>
/// </summary>
/// <remarks>
/// Does not support all property/field types, but at least supports: int, uint, int64, uint64 and string types
/// </remarks>
/// <example>
/// Model.cs:
/// <code>
/// class Model
/// {
///     // Value is compressed upon serialization (stringify) and decompressed on deserialization (objectify)
///     [JsonCompress]
///     public string Token {get;set;} 
///     
///     [JsonCompress]
///     public long ProductId {get;set;}
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class JsonCompressAttribute : JsonConverterAttribute
{
    public JsonCompressAttribute()
    {
    }

    public override JsonConverter CreateConverter(Type typeToConvert)
    {
        if (typeToConvert != SystemType.Int64Type &&
            typeToConvert != SystemType.StringType &&
            typeToConvert != SystemType.IntType &&
            typeToConvert != SystemType.Int16Type &&
            typeToConvert != SystemType.UInt64Type &&
            typeToConvert != SystemType.UIntType)
            throw new Exception("JsonCompress attribute is only allowed on string, (u)int, (u)long, short");

        if (FrameworkConfig.Current.Json.JsonSecureAttributesEnabled)
            return new JsonCompressConverter(this);

        else
        {
            if (typeToConvert == SystemType.IntType)
                return new IntJsonConverter();

            if (typeToConvert == SystemType.Int64Type)
                return new LongJsonConverter();

            return new StringJsonConverter();
        }
    }
}
