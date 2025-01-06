using System.Text.Json.Serialization;

namespace SystemLibrary.Common.Framework.Attributes;

/// <summary>
/// Encrypt and decrypt a property or field on serialization and deserialization
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
///     // Value is encrypted upon serialization (stringify) and decrypted on deserialization (objectify)
///     [JsonEncrypt]
///     public string Token {get;set;} 
///     [JsonEncrypt]
///     public string ProductId {get;set;}
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class JsonEncryptAttribute : JsonConverterAttribute
{
    internal string Key;
    internal string IV;
    internal bool AddedIV;

    public JsonEncryptAttribute(string key = null, string IV = null, bool addedIV = true)
    {
        this.Key = key;
        this.IV = IV;
        this.AddedIV = addedIV;
    }

    public override JsonConverter CreateConverter(Type typeToConvert)
    {
        return new JsonEncryptConverter(this);
    }
}
