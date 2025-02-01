namespace SystemLibrary.Common.Framework.Attributes;

/// <summary>
/// Mark a property as the place holder for the decrypted value of an encrypted config property
/// <para>A place holder property must be an instance, public with get; and set;</para>
/// <para>Attribute is used internally by the Config class within the framework. Whenever the Config class is instantiated, it finds the DecryptAttribute and decrypts accordingly</para>
/// <para>An encrypted config property must be encrypted with the parameterless .Encrypt() method within this framework. The key/iv is either a default or one you've specified. Read more at the StringExtensions.Encrypt() method's documentation</para>
/// </summary>
/// <remarks>
/// The PropertyName must be a property within the same class that this attribute was used, and class must inherit Config to work automatically
/// <para>This class attribute exists to read Config Properties that are public get;set;, but feel free to use ConfigDecrypt attribute yourself as it is not subject for breaking changes in near future</para>
/// The decryption occurs only once for the app life time, at the creation of the Configuration class
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
    /// Set the property name that will be decrypted and the decrypted value is stored within this property
    /// </summary>
    /// <param name="propertyName"></param>
    public ConfigDecryptAttribute(string propertyName = null)
    {
        PropertyName = propertyName;
    }
}
