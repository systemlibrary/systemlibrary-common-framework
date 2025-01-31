namespace SystemLibrary.Common.Framework.Attributes;

/// <summary>
/// Mark a property as the Decrypted value of an Encrypted Configuration Property
/// <para>The property marked must be Public, Instance, Get and Set.</para>
/// <para>This attribute goes hand in hand with Config class, when a Config class is created it looks for which properties has this attribute and decrypts accordingly</para>
/// To decrypt, properties must be encrypted through Encrypt() extension in this library that takes no Key/IV, but the "default" you've specified by configuration/convention. Read more in StringExtensions.Encrypt method how to encrypt.
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
///     public string TokenDecrypted {get;set;} // Naming convention decrypting
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
    /// Set the property name that will be decrypted
    /// </summary>
    /// <param name="propertyName"></param>
    public ConfigDecryptAttribute(string propertyName = null)
    {
        PropertyName = propertyName;
    }
}
