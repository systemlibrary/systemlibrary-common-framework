using Microsoft.Extensions.Configuration;

namespace SystemLibrary.Common.Framework;

internal static class ConfigVariables
{
    internal static IConfigurationRoot AppSettings;

    internal static string _EnvironmentNameLowered;

    internal static string EnvironmentNameLowered
    {
        get
        {
            _EnvironmentNameLowered ??= AppInstance.AspNetCoreEnvironment.ToLower();

            return _EnvironmentNameLowered;
        }
    }

    internal static string[] ConfigurationFilesLowered;

    internal static string[] AppSettingFilesLowered;

    static ConfigVariables()
    {
        var contentRootDirectory = AppInstance.ContentRootPath;

        if (!contentRootDirectory.EndsWith("/", StringComparison.Ordinal) && !contentRootDirectory.EndsWith("\\", StringComparison.Ordinal))
        {
            if (contentRootDirectory.Contains("/", StringComparison.Ordinal))
                contentRootDirectory += "/";
            else
                contentRootDirectory += "\\";
        }

        var allFoundConfigurationFiles = Async.Run(
            () => ConfigFileSearcher.GetConfigurationFilesInFolder(contentRootDirectory, false),
            () => ConfigFileSearcher.GetConfigurationFilesInFolder(contentRootDirectory + "config\\", true),
            () => ConfigFileSearcher.GetConfigurationFilesInFolder(contentRootDirectory + "configs\\", true),
            () => ConfigFileSearcher.GetConfigurationFilesInFolder(contentRootDirectory + "configuration\\", true),
            () => ConfigFileSearcher.GetConfigurationFilesInFolder(contentRootDirectory + "configurations\\", true)
        );

        var tempConfigFiles = allFoundConfigurationFiles
           .SelectMany(arr => arr)
           .Where(FilterValidConfigurationFileNames)
           .ToArray();

        ConfigurationFilesLowered = Array.ConvertAll(tempConfigFiles, s => s.ToLower());

        AppSettingFilesLowered = ConfigurationFilesLowered.Where(FilterAppSettingFiles).ToArray();

        var appSettingsBuilder = new ConfigurationBuilder();

        AddConfigurationFilesAndTransformationFiles(appSettingsBuilder, AppSettingFilesLowered);

        AppSettings = appSettingsBuilder
            .AddEnvironmentVariables()
            .Build();
    }

    static string[] GetConfigurationFilesInFolder(string directoryPath, bool searchRecursively)
    {
        if (!Directory.Exists(directoryPath)) return new string[0];

        string[] files;
        if (!searchRecursively)
            files = Directory.GetFiles(directoryPath, "*.*", SearchOption.TopDirectoryOnly);
        else
            files = Directory.GetFiles(directoryPath, "*.*", SearchOption.AllDirectories);

        if (files == null || files.Length == 0) return new string[0];

        return files
            .Where(x => x.EndsWithAny(StringComparison.OrdinalIgnoreCase, ".json", ".xml", ".config"))
            .ToArray();
    }

    static bool FilterAppSettingFiles(string fileLowered)
    {
        if (fileLowered.IsNot()) return false;

        fileLowered = fileLowered.ToLower();

        return (fileLowered.Contains("\\appsettings.") || fileLowered.Contains("/appsettings.")) && fileLowered.EndsWithAny(StringComparison.OrdinalIgnoreCase, ".json", ".xml", ".config");
    }

    static bool FilterValidConfigurationFileNames(string file)
    {
        if (file.IsNot()) return false;

        file = file.ToLower();

        if (file.Contains(".runtimeconfig.", StringComparison.Ordinal) ||
            file.Contains(".deps.json", StringComparison.Ordinal) ||
            file.Contains("microsoft.visualstudio", StringComparison.Ordinal) ||
            file.Contains("launchSettings.json", StringComparison.Ordinal) ||
            file.Contains("lint.json", StringComparison.Ordinal) ||
            file.Contains("tsconfig.json", StringComparison.Ordinal) ||
            file.Contains("bower.json", StringComparison.Ordinal) ||
            file.Contains("shrinkwrap.json", StringComparison.Ordinal) ||
            file.Contains("build.xml", StringComparison.Ordinal) ||
            file.Contains("editorconfig.json", StringComparison.Ordinal) ||
            file.Contains("app.config", StringComparison.Ordinal) ||
            file.Contains("web.config", StringComparison.Ordinal) ||
            file.Contains("babelrc.json", StringComparison.Ordinal) ||
            file.ContainsAny(StringComparison.Ordinal, "packages.json", "packages.xml", "package.json", "package-lock.json"))
            return false;

        return true;
    }

    internal static void AddConfigurationFilesAndTransformationFiles(ConfigurationBuilder builder, IEnumerable<string> files)
    {
        if (files == null) return;

        foreach (var f in files)
        {
            if (f.IsNot()) continue;

            var extension = Path.GetExtension(f)?.ToLower();
            if (extension == ".json")
            {
                builder.AddJsonFile(f, true, true);
            }
            else if (extension == ".xml")
            {
                if (!f.Contains(".Tests", StringComparison.Ordinal) && f.Contains("\\SystemLibrary.Common.", StringComparison.Ordinal)) continue;

                builder.AddXmlFile(f, true, true);
            }
            else if (extension == ".config")
            {
                if (!f.Contains(".Tests", StringComparison.Ordinal) && f.Contains("\\SystemLibrary.Common.", StringComparison.Ordinal)) continue;

                var data = File.ReadAllText(f);
                if (data.Length > 0)
                {
                    if (data[0] == '<' || data.EndsWith(">", StringComparison.Ordinal))
                        builder.AddXmlFile(f, true, true);
                    else
                        builder.AddJsonFile(f, true, true);
                }
            }
        }
    }
}

internal static class ConfigFileSearcher
{
    internal static string[] GetConfigurationFilesInFolder(string directoryPath, bool searchRecursively)
    {
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
}