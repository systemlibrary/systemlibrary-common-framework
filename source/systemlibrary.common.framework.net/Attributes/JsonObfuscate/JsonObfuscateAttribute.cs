using System.Text.Json.Serialization;

namespace SystemLibrary.Common.Framework.Attributes;

/// <summary>
/// Obfuscate and deobfuscate a property or field on serialization and deserialization
/// <para>Useful when you want to hide productId's or similar in Frontend part of your application. Avoids having int's or ID's in the frontend, for attackers wanting to brute force endpoints taking INTs</para>
/// </summary>
/// <remarks>
/// Does not support all property/field types, but at least supports: int, uint, int64, uint64 and string types
/// </remarks>
/// <example>
/// Model.cs:
/// <code>
/// class Model
/// {
///     // Value is obfuscted upon serialization (stringify) and deobfuscated on deserialization (objectify)
///     [JsonObfuscate]
///     public string Token {get;set;} 
///     [JsonObfuscate]
///     public string ProductId {get;set;}
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class JsonObfuscateAttribute : JsonConverterAttribute
{
    internal int Salt;

    public JsonObfuscateAttribute(int salt = 77)
    {
        Salt = salt;
    }

    public override JsonConverter CreateConverter(Type typeToConvert)
    {
        return new ObfuscateJsonConverter(this);
    }
}
