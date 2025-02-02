namespace SystemLibrary.Common.Framework.Attributes;

/// <summary>
/// Marks a property as the placeholder for the decrypted value of an encrypted config property.
/// <para>The placeholder property must be an instance, public with both get and set accessors.</para>
/// <para>This attribute is used internally by the Config class within the framework. When the Config class is instantiated, it locates the DecryptAttribute and performs decryption accordingly.</para>
/// <para>An encrypted config property must be encrypted using the parameterless .Encrypt() method within this framework. The key/IV is either the default or one that you've specified. See the documentation for the StringExtensions.Encrypt() method for more details.</para>
/// </summary>
/// <remarks>
/// The PropertyName must refer to a property within the same class that this attribute is applied to, and the class must inherit from Config for automatic decryption to work.
/// <para>This class attribute exists to read Config Properties that are public get;set;, but feel free to use ConfigDecrypt attribute yourself as it is not subject for breaking changes in near future.</para>
/// Decryption occurs once during the application's lifetime, specifically during the creation of the Configuration class.
/// </remarks>
/// <example>
/// apiConfig.json:
/// <code>
/// {
///    token: "An encrypted value of the token, store it safely in git as it is encrypted by a key/IV only you know!"
/// }
/// </code>
/// ApiConfig.cs:
/// <code>
/// class ApiConfig : Config
/// {
///    public string Token {get;set;} //Encrypted value...
///    
///     public string TokenDecrypt {get;set;} // Naming convention decrypting
///     
///     [ConfigDecrypt(propertyName="Token")]
///     public string TokenDec {get;set;} // Attribute convention decrypting
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class ConfigDecryptAttribute : Attribute
{
    public string PropertyName;

    /// <summary>
    /// Sets the property name to be decrypted, with the decrypted value stored within this property.
    /// </summary>
    /// <param name="propertyName"></param>
    public ConfigDecryptAttribute(string propertyName = null)
    {
        PropertyName = propertyName;
    }
}
