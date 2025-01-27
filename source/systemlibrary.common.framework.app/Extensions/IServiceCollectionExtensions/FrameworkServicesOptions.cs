﻿using System.Reflection;

using Microsoft.AspNetCore.Mvc.Razor;

namespace SystemLibrary.Common.Framework.App.Extensions;

/// <summary>
/// Web App Services Collection Options
/// </summary>
/// <remarks>
/// </remarks>
/// <example>
/// Inside your startup.cs/program.cs...
/// <code>
/// public class CustomViewLocations : IViewLocationExpander
/// {
///     //...implement the interface
///     public IEnumerable&lt;string&gt; ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable&lt;string&gt; viewLocations)
///     {
///         return new string[] {   
///             "~/Folder2/{0}/Index.cshtml"
///         }
///     }
/// }
/// 
/// public void ConfigureServices(IServiceCollection services)
/// {
///     var options = new FrameworkServicesOptions();
///     
///     options.UseControllers = false;
///     //Note: two ways to add view locations, either through an Expander class
///     options.ViewLocationExpander = new CustomViewLocations();
///     
///     //Or directly adding a string array
///     options.ViewLocations = new string[] {
///         "~/Folder/{0}/Index.cshtml",
///         "~/Folder/{1}/{0}.cshtml"
///     }
///     
///     app.AddFrameworkServices(options);
/// }
/// </code>
/// </example>
public class FrameworkServicesOptions : BaseOptions
{
    /// <summary>
    /// Set to true to add MVC services
    /// </summary>
    public bool UseMvc = true;

    /// <summary>
    /// Add application assembly as a 'part' so controllers within your application assembly are tried matching against requests
    /// </summary>
    public bool AddApplicationAsPart = true;

    /// <summary>
    /// Add multiple assemblies as a 'part' so controllers within the assemblies are tried matched against requests
    /// </summary>
    public Assembly[] ApplicationParts = null;

    /// <summary>
    /// Enabled re-compilation of .cshtml files upon saving .cshtml files
    /// <list>
    /// <item>- Avoids the need of a re-compilation of whole application for one small view change</item>
    /// <item>- Package 'System.Security.Cryptography.Pkcs' is required, not a direct dependency, but through SystemLibrary.Common.Framework. If you turn this on and it throws? Add the right version of System.Security.Cryptography.Pkcs</item>
    /// </list>
    /// </summary>
    public bool AddRazorRuntimeCompilationOnChange = true;

    /// <summary>
    /// Pass in an object that implements the interface if you want to extend View Locations
    /// <para>Another option is to simply set 'ViewLocations' variable or 'AreaViewLocations'</para>
    /// </summary>
    public IViewLocationExpander ViewLocationExpander = null;

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
    /// var options = new FrameworkServicesOptions();
    /// options.ViewLocations = new string[] { "~/Pages/{2}/{1}/{0}.cshtml" }
    /// </code>
    /// </example>
    public string[] ViewLocations = null;

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
    /// var options = new FrameworkServicesOptions();
    /// options.ViewLocations = new string[] { "~/Pages/{2}/{1}/{0}.cshtml" }
    /// </code>
    /// </example>
    public string[] AreaViewLocations;

    /// <summary>
    /// Generate and enable data protection with a 31 days rotating key files
    /// <para>Parent folder of 'ContentRoot' will be used as the destination for the key files</para>
    /// <para>Key files older than 60 days will be deleted automatically, meaning older data encrypted is not possible to decrypt, for instance old login cookies, which forces people to relogin every 60 days at minimum</para>
    /// - string extension methods EncryptUsingKeyRing and DecryptUsingKeyRing will use the generated files internally
    /// <para>- cookies read over http will be encrypted and decrypted with the key file, if you host your app over several instances, they must all share the same key of course</para>
    /// </summary>
    public bool UseDataProtectionPolicy = false;

    /// <summary>
    /// Add an internal logger that forwards errors to the ILogWriter of your own choice
    /// <para>Standard output is forwarded to your own ILogWriter</para>
    /// </summary>
    public bool ForwardStandardLogging = false;

    /// <summary>
    /// Set to true to add ResponseCaching services
    /// </summary>
    public bool UseResponseCaching = true;

    /// <summary>
    /// Allow synchronous IO, an IIS setting
    /// </summary>
    public bool AllowSynchronousIO = true;

    /// <summary>
    /// Registers special Type Converters to the System.ComponentModel.TypeDescriptor
    /// <para>For instance: it registers the .ToEnum() method as the primary method to convert a String to an Enum, so EnumValue and EnumText works</para>
    /// </summary>
    /// <remarks>
    /// Note: This is used when converting data in a response to a Model within a GET or POST method for instance, do not mix it with JSON Serialization, thats something else
    /// </remarks>
    public bool UseExtendedEnumModelConverter = true;
}