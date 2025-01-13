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

        var keyManagementOptionsType = typeof(IConfigureOptions<KeyManagementOptions>);

        var keyManagementOptions = services.FirstOrDefault(sd => sd.ServiceType == keyManagementOptionsType);

        if (keyManagementOptions != null)
        {
            Log.Warning("UseAutomaticDataProtectionPolicy is set to True, but AddDataProtection() has already been registered, doing nothing...");

            return services;
        }

        var appName =  AppInstance.AppName;

        var cryptationKey = CryptationKey.Current;

        if (cryptationKey == null)
            Debug.Write("Dummy if statement to trigger KeyFileFullName being set through Lock() mechanism in GetKey()");

        var keyFileFullName = CryptationKey._KeyFileFullName;

        if (keyFileFullName.Is())
        {
            var directory = Path.GetDirectoryName(keyFileFullName);

            Debug.Log("Key file already exists at: " + directory);

            return services.AddDataProtection()
                .DisableAutomaticKeyGeneration()
                .PersistKeysToFileSystem(new DirectoryInfo(directory))
                .SetApplicationName(appName)
                .SetDefaultKeyLifetime(TimeSpan.FromDays(365 * 1000))
                .Services;
        }
        else
        {
            var directory = new DirectoryInfo(AppInstance.ContentRootPath).Parent;

            Debug.Log("Auto-generating key file at content root's parent: " + directory.FullName);

            return services.AddDataProtection()
                .PersistKeysToFileSystem(directory)
                .SetApplicationName(appName)
                .SetDefaultKeyLifetime(TimeSpan.FromDays(365 * 1000))
                .Services;
        }
    }
}