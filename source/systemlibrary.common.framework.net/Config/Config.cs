using System.Reflection;

using Microsoft.Extensions.Configuration;

using SystemLibrary.Common.Framework.Attributes;

namespace SystemLibrary.Common.Framework;

/// <summary>
/// Class for loading and reading configuration files (XML, JSON, or config) as a class, applying transformations when present, and decrypting encrypted properties.
/// <para>Configurations can be placed in the following locations:</para>
/// ~/*.json, ~/*.xml, ~/Configs/**.[json|xml], or ~/Configurations/**.[json|xml]
/// <para>Or appended to your existing appSettings.json file.</para>
/// Transformations are applied based on the ASPNETCORE_ENVIRONMENT variable passed to your application.
/// <para>Recommended places to set ASPNETCORE_ENVIRONMENT:</para>
/// - launchSettings.json when using IIS Express
/// <para>- web.config if using IIS</para>
/// - mstest.runsettings if running unit tests
/// <para>- command-line with --configuration if running as an executable.</para>
/// </summary>
/// <remarks>
/// The current instance of the Config object is a singleton and is loaded only once.
/// <para>For an example, see the EnvironmentConfig.Name property, which provides details on where/how to set the environment per application type.</para>
/// Encrypted properties such as ApiToken {get; set;} can be decrypted automatically by creating a corresponding ApiTokenDecrypt {get; set;} property.
/// <para>The property must be public, of type string, and marked with get; set;.</para>
/// <para>This follows the convention of specifying the suffix "Decrypt" or using the [ConfigDecrypt] attribute on a property.</para>
/// <para>Environment variables like UserName are only added when reading from appSettings, not from your custom configuration files.</para>
/// WARNING: The generic type T cannot be a nested class
/// </remarks>
/// <example>
/// - Create new file '~/TestConfig.json'
/// - Can also be placed under ~/Configs/ or ~/Configurations/
/// <code class="language-xml hljs">
/// {
///     "Name": "Hello World",
///     "Number": 1234,
///     
///     "Options": {
///         "Url": "https://....",
///     },
///     "ValidPhoneNumbers": [0,1,2,3]
///     "password": "/HjLI26feCvCwaBOmXfu6ZFK1KLR33YbAdDZIlzuM5I=",
/// }
/// </code>
/// 
/// - Create C# class with same name as your newly created json file and inherit Config&lt;&gt;: 
/// <code class="language-csharp hljs">
/// public class TestConfig : Config&lt;TestConfig&gt; 
/// {
///    public string Name { get; set; }
///    
///    public int Number { get; set;}
/// 
///    public ApiOptions Options { get; set; }
///    
///    public int[] ValidPhoneNumbers { get; set; }
///    
///    public class Password { get; set; } // Contains encrypted value
///    public class PasswordDecrypt { get; set; } // Contains decrypted Password at runtime
/// }
/// 
/// public class ApiOptions 
/// {
///     public string Url { get; set; }
/// }
/// </code>
/// 
/// Usage:
/// <code class="language-csharp hljs">
/// var testConfig = TestConfig.Current;
/// var name = testConfig.Name;
/// // name is now Hello World
/// </code>
/// 
/// Add transformation per 'environmnet' to our newly created TestConfig.json
/// - Add transformation for an environment, lets call it 'dev'
/// - Create TestConfig.dev.json file, place it in same folder as TestConfig.json
///     - Visual Studio should mark the new file as 'IsTransformFile=true' and 'DependentUpon=TestConfig.json'
///         - If not, try dragging "TestConfig.dev.json" in under "TestConfig.json" via Solution Explorer
///         
/// - Define only variables that we want to transform:
/// <code class="language-xml hljs">
/// {
///     "Name": "Hello Dev!",
/// }
/// </code>
/// 
/// Here are three ways of specifying the environment, by either launchSettings.json, web.config or mstest.runsettings.
/// 
/// 1 launchSettings.json with IIS:
/// <code class="language-csharp hljs">
/// { 
/// 	...
/// 	{
/// 		"profiles": {
/// 			"IIS": {
/// 				"environmentVariables": {
/// 					"ASPNETCORE_ENVIRONMENT": "Dev",
/// 				}
/// 			}
/// 		}
/// 	}
/// 	...
/// }
/// </code>
/// 
/// 2 web.config with IIS or IISExpress: 
/// <code class="language-csharp hljs">
/// &lt;configuration&gt;
///   &lt;location path = "." inheritInChildApplications="false"&gt;
/// 	&lt;/system.webServer&gt;
/// 	  &lt;aspNetCore processPath = "bin\Demo.exe" arguments="" stdoutLogEnabled="false" hostingModel="inprocess"&gt;
///         &lt;environmentVariables&gt;
///           &lt;environmentVariable name = "ASPNETCORE_ENVIRONMENT" value="Dev" /&gt;
///         &lt;/environmentVariables&gt;
///       &lt;/aspNetCore&gt;
///     &lt;/system.webServer&gt;
///   &lt;/location&gt;
/// &lt;/configuration&gt;
/// </code>
/// 
/// 3 mstest.runsettings if running through Test Explorer (unit tests):
/// - Note: add mstest.runsettings to your csproj-variable: 'RunSettingsFilePath'
/// - Tip: View source code of SystemLibrary.Common.Framework.Tests inside the repo SystemLibrary.Common.Framework on github
/// <code class="language-csharp hljs">
/// &lt;RunSettings&gt;
///   &lt;RunConfiguration&gt;
///       &lt;EnvironmentVariables&gt;
///           &lt;ASPNETCORE_ENVIRONMENT&gt;Dev&lt;/ASPNETCORE_ENVIRONMENT&gt;
/// </code>
/// 
/// Usage:
/// - Assume IISExpress and web.config setup above:
/// <code class="language-csharp hljs">
/// var testConfig = TestConfig.Current;
/// var name = testConfig.Name;
/// // name is now equal to 'Hello Dev!', which is our transformed value
/// </code>
/// </example>
/// <typeparam name="T">T is the class inheriting Config&lt;&gt;, also referenced as 'self'. Note that T cannot be a nested class</typeparam>
public abstract partial class Config<T> where T : class
{
    static List<string[]> ConfigurationFiles = ConfigAsyncDirectorySearcher.Search();

    static Config()
    {
        var configuration = ConfigLoader<T>.Load();
        try
        {
            // Activate the instance to create it's static properties and fields
            // Note: but I cannot do this anymore, as that sets the static properties to its default configurations...
            //_Current = Activator.CreateInstance<T>();
            Current = configuration.Get<T>(opt =>
            {
                opt.ErrorOnUnknownConfiguration = false;
            });
        }
        catch
        {
            try
            {
                Current = Activator.CreateInstance<T>();
                Current = null;
                Current = configuration.Get<T>(opt =>
                {
                    opt.ErrorOnUnknownConfiguration = false;
                });
            }
            catch (Exception ex)
            {
                Debug.Write(ex);
            }
        }

        var type = typeof(T);

        if (Current == null)
            throw new Exception(type.Name + " could not be created. A '" + type.Name + ".json' file must exist and it cannot be empty. File mustbe in ~/Configs or ~/Configurations or a section in root of appSettings.json named '" + type.Name + "' is also supported.");

        try
        {
            DecryptPublicGetSetProperties(Current, type);
        }
        catch
        {
        }

        try
        {
            SetPublicEnumFields(configuration, Current, type);
        }
        catch
        {
        }
    }

    /// <summary>
    /// Gets the current configuration as a singleton object, always instantiated, thread-safe, and should not throw exceptions.
    /// </summary>
    public static T Current;

    static void DecryptPublicGetSetProperties(object instance, Type type)
    {
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty)?.Where(prop => prop.PropertyType == SystemType.StringType);

        if (properties == null) return;

        foreach (var property in properties)
        {
            if (property == null) continue;

            if (!property.CanWrite || !property.CanRead) continue;

            var isEligibleForDecryption = property.Name.EndsWith("Decrypt", StringComparison.Ordinal);

            var attribute = property.GetCustomAttribute<ConfigDecryptAttribute>();

            // Not a decrypt property
            if (!isEligibleForDecryption && attribute == null) continue;

            // Name convention mismatch, but propertyName not specified, not able to decrypt
            if (!isEligibleForDecryption && attribute.PropertyName.IsNot())
            {
                Debug.Log("Config did not decrypt " + property.Name + " as DecryptAttribute 'PropertyName' is null/blank");
                continue;
            }

            var encryptedPropertyName = attribute?.PropertyName ??
                property.Name.ReplaceAllWith("", "Decrypt");

            var encryptedProperty = FindEncryptedProperty(properties, encryptedPropertyName);

            if (encryptedProperty == null)
            {
                Debug.Log("Config did not decrypt " + property.Name + " as the encrypted property was not found: " + encryptedPropertyName);
                continue;
            }

            var cipherText = encryptedProperty.GetValue(instance) as string;

            if (cipherText == null)
            {
                Debug.Log("Config property " + encryptedProperty.Name + " is null, decrypting nothing, continue...");
                continue;
            }

            var decryptedValue = cipherText.Decrypt();

            if (decryptedValue != null)
            {
                property.SetValue(instance, decryptedValue);
            }
            else
            {
                Debug.Log("Decrypting " + encryptedProperty.Name + " returned null");
            }
        }
    }

    static void SetPublicEnumFields(IConfiguration configuration, object instance, Type type)
    {
        if (instance == null) return;
        if (configuration == null) return;

        var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetField | BindingFlags.SetField);

        if (fields == null) return;

        foreach (var field in fields)
        {
            var fieldType = field.FieldType;
            if (fieldType.IsEnum)
            {
                var value = configuration[field.Name];
                if (value != null)
                {
                    field.SetValue(Current, value.ToEnum(fieldType));
                }
            }
        }
    }
    static PropertyInfo FindEncryptedProperty(IEnumerable<PropertyInfo> properties, string encryptedPropertyName)
    {
        return properties.FirstOrDefault(x => x.Name == encryptedPropertyName);
    }
}

internal static class ConfigAsyncDirectorySearcher
{
    static List<string[]> ConfigurationFiles;

    internal static List<string[]> Search()
    {
        if (ConfigurationFiles == null)
        {
            var contentRootDirectory = AppInstance.ContentRootPath;

            if (!contentRootDirectory.EndsWith("/", StringComparison.Ordinal) && !contentRootDirectory.EndsWith("\\", StringComparison.Ordinal))
            {
                if (contentRootDirectory.Contains("/", StringComparison.Ordinal))
                    contentRootDirectory += "/";
                else
                    contentRootDirectory += "\\";
            }

            ConfigurationFiles = Async.Parallel(
                () => ConfigFileSearcher.GetConfigurationFilesInFolder(contentRootDirectory, false),
                () => ConfigFileSearcher.GetConfigurationFilesInFolder(contentRootDirectory + "configs\\", true),
                () => ConfigFileSearcher.GetConfigurationFilesInFolder(contentRootDirectory + "configurations\\", true)
            );
        }
        return ConfigurationFiles;
    }
}