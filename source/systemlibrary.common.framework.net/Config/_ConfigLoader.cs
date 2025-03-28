using Microsoft.Extensions.Configuration;

namespace SystemLibrary.Common.Framework;

partial class Config<T>
{
    internal static class ConfigLoader<TConf> where TConf : class
    {
        internal static IConfiguration Load()
        {
            var type = typeof(T);

            var configName = type.Name;

            var files = new List<string>();
            // Find the "master" config file for the current config T

            foreach (var configFile in ConfigFileLoader.ConfigurationFiles)
            {
                if (configFile.IsNot()) continue;

                if (!configFile.Contains(configName + ".", StringComparison.OrdinalIgnoreCase)) continue;

                var values = configFile.Split('.');

                if (values != null && values.Length > 1 && values[^2].Contains(configName, StringComparison.OrdinalIgnoreCase))
                {
                    if (values[^2].Contains("\\" + configName, StringComparison.OrdinalIgnoreCase) ||
                        values[^2].Contains("/" + configName, StringComparison.OrdinalIgnoreCase))
                    {
                        files.Add(configFile);
                    }
                }
            }

            // Add transformation file for environment if found
            if (ConfigFileLoader.environmentNameLowered.Is())
            {
                var configTransformationName = configName + "." + ConfigFileLoader.environmentNameLowered + ".";

                foreach (var configFile in ConfigFileLoader.ConfigurationFiles)
                    if (configFile.Contains(configTransformationName, StringComparison.OrdinalIgnoreCase) &&
                        !files.Contains(configFile))
                    {
                        files.Add(configFile);
                    }
            }

            if (files.Count > 0)
            {
                var builder = new ConfigurationBuilder();

                ConfigFileLoader.AddConfigurationFilesAndTransformationFiles(builder, files);

                //TODO: Why should XML add env paths and not json? or any at all? string username will then always be overridden with 'computer user name' for instance
                var isXml = files.Where(x => x.EndsWith(".xml", StringComparison.Ordinal)).Count();
                if (isXml > 0)
                {
                    return builder
                        .AddEnvironmentVariables()
                        .Build();
                }
                else
                {
                    return builder
                        .Build();
                }
            }

            return ConfigFileLoader.ConfigRoot;
        }
    }
}
