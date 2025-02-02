using System.ComponentModel;

using Microsoft.Extensions.Configuration;

namespace SystemLibrary.Common.Framework;

internal static class ConfigFileSearcher
{
    internal static string[] GetConfigurationFilesInFolder(string directoryPath, bool searchRecursively)
    {
        try
        {
            if (directoryPath.IsNot()) return [];

            if (!Directory.Exists(directoryPath)) return [];

            string[] files;

            if (!searchRecursively)
                files = Directory.GetFiles(directoryPath, "*.*", SearchOption.TopDirectoryOnly);
            else
                files = Directory.GetFiles(directoryPath, "*.*", SearchOption.AllDirectories);

            if (files == null || files.Length == 0) return [];

            return files
                .Where(x => x.EndsWithAny(StringComparison.OrdinalIgnoreCase, ".json", ".xml", ".config"))
                .ToArray();
        }
        catch
        {
            return [];
        }
    }
}