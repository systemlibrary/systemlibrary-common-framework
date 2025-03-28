using Microsoft.Extensions.Configuration;

namespace SystemLibrary.Common.Framework;

internal static class ConfigFileLoader
{
    // ConfigurationRoot is usually 'appsettings.json' and its values if it exists
    internal static IConfigurationRoot ConfigRoot;

    internal static string environmentNameLowered = AppInstance.AspNetCoreEnvironment.ToLower();

    internal static string[] ConfigurationFiles;

    static ConfigFileLoader()
    {
        ConfigEnumConvertRegister.Register();

        var appSettingsBuilder = new ConfigurationBuilder();

        var allConfigurationFiles = ConfigAsyncDirectorySearcher.Search();

        if (allConfigurationFiles != null)
        {
            ConfigurationFiles = allConfigurationFiles
              .SelectMany(arr => arr)
              .Where(FilterValidConfigurationFileNames)
              .ToArray();

            var appsettingFiles = ConfigurationFiles.Where(FilterByAppsettings).ToArray();

            AddConfigurationFilesAndTransformationFiles(appSettingsBuilder, appsettingFiles);
        }
        else
            ConfigurationFiles = [];

        ConfigRoot = appSettingsBuilder
            .AddEnvironmentVariables()
            .Build();
    }

    static bool FilterByAppsettings(string configFile)
    {
        if (configFile.IsNot()) return false;

        return (configFile.Contains("\\appsettings.", StringComparison.OrdinalIgnoreCase) ||
            configFile.Contains("/appsettings.", StringComparison.OrdinalIgnoreCase)) &&
            configFile.EndsWithAny(StringComparison.OrdinalIgnoreCase, ".json", ".xml", ".config");
    }

    static bool FilterValidConfigurationFileNames(string file)
    {
        if (file.IsNot()) return false;

        file = file.ToLower();

        if (file.Contains(".runtimeconfig.", StringComparison.Ordinal) ||
            file.Contains("AppManifest.", StringComparison.OrdinalIgnoreCase) ||
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
