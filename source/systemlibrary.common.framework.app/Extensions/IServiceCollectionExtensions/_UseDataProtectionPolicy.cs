using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace SystemLibrary.Common.Framework.App.Extensions;

partial class IServiceCollectionExtensions
{
    public static IServiceCollection UseDataProtectionPolicy(this IServiceCollection services, FrameworkOptions options)
    {
        if (!options.UseDataProtectionPolicy) return services;

        var keyManagementOptionsType = typeof(IConfigureOptions<KeyManagementOptions>);

        var keyManagementOptions = services.FirstOrDefault(sd => sd.ServiceType == keyManagementOptionsType);

        if (keyManagementOptions != null)
        {
            Log.Warning("UseDataProtectionPolicy is set to True, but AddDataProtection() has already been registered, doing nothing...");

            return services;
        }

        var root = new DirectoryInfo(AppInstance.ContentRootPath);

        var keyRingDirectory = new DirectoryInfo(Path.Combine(root.FullName, "App_Data", "DataProtectionKeys"));

        var keyRingFiles = GetValidKeyFiles(keyRingDirectory);

        var appName = AppInstance.AppName;

        if (keyRingFiles.Count > 0)
        {
            Debug.Log("[UseDataProtectionPolicy] Key file already exists at: " + keyRingDirectory.FullName);

            return services.AddDataProtection()
                .DisableAutomaticKeyGeneration()
                .PersistKeysToFileSystem(keyRingDirectory)
                .SetApplicationName(appName)
                .SetDefaultKeyLifetime(TimeSpan.FromDays(365 * 100))
                .Services;
        }
        else
        {
            Debug.Log("[UseDataProtectionPolicy] Generating key file at content root's parent: " + keyRingDirectory.FullName);

            return services.AddDataProtection()
                .PersistKeysToFileSystem(keyRingDirectory)
                .SetApplicationName(appName)
                .SetDefaultKeyLifetime(TimeSpan.FromDays(365*100))
                .Services;
        }
    }

    static List<FileInfo> GetValidKeyFiles(DirectoryInfo directory)
    {
        var validFiles = new List<FileInfo>();

        var files = directory.GetFiles("*.xml", SearchOption.TopDirectoryOnly);

        if (files == null || files.Length == 0) return validFiles;

        foreach (var file in files)
        {
            if (file.FullName.Contains("key-") && file.Length > 1)
            {
                validFiles.Add(file);
            }
        }

        return validFiles;
    }
}