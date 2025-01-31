using System.Text.Json.Serialization;

namespace SystemLibrary.Common.Framework.Attributes;

/// <summary>
/// Compress and decompress a property or field on serialization and deserialization
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
        return new JsonCompressConverter(this);
    }
}
