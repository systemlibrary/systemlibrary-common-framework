using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace SystemLibrary.Common.Framework;

partial class CryptationKey
{
    internal static string TryGetKeyFromKeyFile()
    {
        if (_KeyFileFullName.Is()) return Path.GetFileName(_KeyFileFullName);

        var keyManagementOptions = ServiceProviderInstance.Current.GetService<IOptions<KeyManagementOptions>>();

        var keyDirectory = (keyManagementOptions?.Value?.XmlRepository as FileSystemXmlRepository)?.Directory?.FullName;

        if (keyDirectory.IsNot()) keyDirectory = EnvironmentConfig.Current.ContentRootPath;

        if (keyDirectory.IsNot()) return null;

        _KeyFileFullName = GetKeyFileFullName(keyDirectory);

        if (_KeyFileFullName.IsNot())
        {
            var parent = new DirectoryInfo(keyDirectory).Parent;

            var count = 13;

            while (_KeyFileFullName.IsNot() && count > 0)
            {
                if (parent == null) return null;

                count--;

                _KeyFileFullName = GetKeyFileFullName(parent.FullName);

                if (_KeyFileFullName.IsNot()) 
                    parent = parent.Parent;
            }

            if (_KeyFileFullName.Is())
                Debug.Log("Found key file in " + parent.FullName);
        }
        else
        {
            if (_KeyFileFullName.Is())
                Debug.Log("Found key file in " + keyDirectory);
        }

        if (_KeyFileFullName.IsNot()) return null;

        return Path.GetFileName(_KeyFileFullName);
    }

    static string GetKeyFileFullName(string keyDirectory)
    {
        var fileNames = Directory.GetFiles(keyDirectory, "*.xml", SearchOption.TopDirectoryOnly);

        if (fileNames == null || fileNames.Length == 0) return null;

        // Order to preserve the key file used if found multiple, makes sure oldest is returned always
        if (fileNames.Length > 1)
        {
            fileNames = fileNames
                .OrderBy(file => file.Length)
                .ThenBy(file =>
                {
                    var creationTime = File.GetCreationTime(file);
                    return creationTime == DateTime.MinValue
                        ? File.GetLastWriteTime(file)
                        : creationTime;
                })
                .ToArray();
        }

        foreach (var fullFileName in fileNames)
        {
            var validated = ValidateFileContent(fullFileName);

            if (validated != null) return validated.Replace("key-", "");
        }
        return null;
    }

    static string ValidateFileContent(string fullFileName)
    {
        if (fullFileName.Length <= 12) return null;

        if (!fullFileName.Contains("key-")) return null;

        return fullFileName;
    }
}
