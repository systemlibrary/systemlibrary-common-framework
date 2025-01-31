using System;
using System.IO;
using System.Linq;
using System.Reflection;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace SystemLibrary.Common.Framework.App.Extensions;

partial class IServiceCollectionExtensions
{
    public static IServiceCollection UseAutomaticDataProtectionPolicy(this IServiceCollection services, FrameworkServicesOptions options)
    {
        if (!options.UseAutomaticDataProtectionPolicy) return services;

        bool AlreadyRegisteredDataProtectionServices()
        {
            var keyOptions = typeof(IConfigureOptions<KeyManagementOptions>);

            var found = services.FirstOrDefault(sd => sd.ServiceType == keyOptions);

            return found != null;
        }

        if (AlreadyRegisteredDataProtectionServices())
        {
            Log.Warning("UseAutomaticDataProtectionPolicy is set to True, but AddDataProtection() has already been registered as a service, doing nothing...");

            return services;
        }

        var type = Type.GetType("SystemLibrary.Common.Framework.CryptationKey, SystemLibrary.Common.Framework.Net");

        if (type == null)
            throw new Exception("SystemLibrary.Common.Framework.CryptationKey is not loaded or type is renamed in version you are using");

        var method = type.GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
            .Where(x => x.Name == "TryGetKeyFromDataRingKeyFile")
            .FirstOrDefault();


        if (method == null)
            throw new Exception("Method 'TryGetKeyFromDataRingKeyFile' is renamed or do not exist from type SystemLibrary.Common.Net.CryptationKey");

        var keyFileName = (string)method.Invoke(null, new object[0]);

        var fields = type.GetFields(BindingFlags.Static | BindingFlags.NonPublic);

        var keyFileFullNameField = fields.Where(x => x.Name == "_KeyFileFullName").FirstOrDefault();

        if (keyFileFullNameField == null)
            throw new Exception("Private static string '_KeyFileFullName' is renamed or do not exist from type SystemLibrary.Common.Net.CryptationKey");

        var appName = "AppName" +
              Assembly.GetEntryAssembly()?
              .GetName()?
              .Name?
              .ToLower()?
              .ReplaceAllWith("-", ",", ".", " ", "=", "/", "\\")?
              .MaxLength(32) +
              Assembly.GetCallingAssembly()?
              .GetName()?
              .Name?
              .ToLower()
              .ReplaceAllWith("-", ",", ".", " ", "=", "/", "\\")?
              .MaxLength(4);

        if (keyFileName.Is())
        {
            var keyFileFullName = keyFileFullNameField.GetValue(null).ToString();

            var directory = Path.GetDirectoryName(keyFileFullName);

            Debug.Log("Key file already exists at: " + directory);

            return services.AddDataProtection()
                .DisableAutomaticKeyGeneration()
                .PersistKeysToFileSystem(new DirectoryInfo(directory))
                .SetApplicationName(appName)
                .SetDefaultKeyLifetime(TimeSpan.FromDays(365 * 200))
                .Services;
        }
        else
        {
            keyFileFullNameField.SetValue(null, null);

            var directory = new DirectoryInfo(EnvironmentConfig.Current.ContentRootPath).Parent;

            Debug.Log("Auto-generating key file at content root's parent: " + directory.FullName);

            return services.AddDataProtection()
                .PersistKeysToFileSystem(directory)
                .SetApplicationName(appName)
                .SetDefaultKeyLifetime(TimeSpan.FromDays(365 * 1000))
                .Services;
        }
    }
}