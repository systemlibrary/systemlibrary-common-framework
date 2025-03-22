using System.Reflection;

using Microsoft.AspNetCore.Mvc.Razor;

namespace SystemLibrary.Common.Framework;

partial class FrameworkOptions
{
    /// <summary>
    /// Add multiple assemblies as a 'part' so controllers within the assemblies are tried matched against requests
    /// </summary>
    public Assembly[] ApplicationParts = null;

    /// <summary>
    /// Pass in a string array of area view location formats
    /// <para>Example:</para>
    /// AreaViewLocations = new string[] { "~/Area/{2}/{1}/{0}.cshtml" };
    /// </summary>
    /// <remarks>
    /// This sets area view locations
    /// </remarks>
    /// <example>
    /// Simple example:
    /// <code>
    /// var options = new FrameworkOptions();
    /// options.ViewLocations = new string[] { "~/Pages/{2}/{1}/{0}.cshtml" }
    /// </code>
    /// </example>
    public string[] AreaViewLocations;

    /// <summary>
    /// Pass in a string array of view location formats
    /// <para>Example:</para>
    /// ViewLocations = new string[] { "~/Pages/{1}/{0}.cshtml" };
    /// </summary>
    /// <remarks>
    /// This sets non-area view locations
    /// </remarks>
    /// <example>
    /// Simple example:
    /// <code>
    /// var options = new FrameworkOptions();
    /// options.ViewLocations = new string[] { "~/Pages/{2}/{1}/{0}.cshtml" }
    /// </code>
    /// </example>
    public string[] ViewLocations = null;

    /// <summary>
    /// Forwards all Microsoft.Extensions.Logging.ILogger log events to your custom ILogWriter.
    /// <para>If enabled, an internal LogProvider is added to capture and route all logs.</para>
    /// </summary>
    public bool UseForwardILogger = false;

    /// <summary>
    /// Generates a single key file once, reusable across all environments, which expires in 100 years.
    /// <para>Parent folder of 'ContentRoot' will be used as the destination for the key file</para>
    /// - string extension methods EncryptUsingKeyRing and DecryptUsingKeyRing will use the generated file internally
    /// <para>- cookies read over http will be encrypted and decrypted with the key file, if you host your app over several instances, they must all share the same key of course</para>
    /// <para>You can enable it and copy the key file out to the pipeline and other environments, and gitignore the file as it should be stored seperately</para>
    /// </summary>
    public bool UseDataProtectionPolicy = false;

    /// <summary>
    /// Set to true to add ResponseCaching services
    /// </summary>
    public bool UseResponseCaching = true;

    /// <summary>
    /// Registers special Type Converters to the System.ComponentModel.TypeDescriptor
    /// <para>For instance: it registers the .ToEnum() method as the primary method to convert a String to an Enum, so EnumValue and EnumText works</para>
    /// </summary>
    /// <remarks>
    /// Note: This is used when converting data in a response to a Model within a GET or POST method for instance, do not mix it with JSON Serialization, thats something else
    /// </remarks>
    public bool UseExtendedEnumModelConverter = true;

    /// <summary>
    /// Pass in an object that implements the interface if you want to extend View Locations
    /// <para>Another option is to simply set 'ViewLocations' variable or 'AreaViewLocations'</para>
    /// </summary>
    public IViewLocationExpander ViewLocationExpander = null;

    /// <summary>
    /// Set directory path which contains the 'frameworkenc-SomePassword.key' file, and the filename will be used a the Global Encryption Key throughout the App whenever you invoke Encrypt or Decrypt
    /// <para>For instance on windows it could be outside the application: C:\src\keys\</para>
    /// <para>Or it can be a relative folder within your application that is protected and not served, like app_data for both windows and linux support: ./app_data/</para>
    /// <para>Relative folders must start with ./</para>
    /// </summary>
    public string EncKeyDir = null;
}