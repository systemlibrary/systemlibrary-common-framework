using Microsoft.Extensions.Configuration;

namespace SystemLibrary.Common.Framework;

partial class Config<T>
{
    internal static class ConfigLoader<TConf> where TConf : class
    {
        internal static IConfiguration Load()
        {
            var type = typeof(T);

            var configNameLowered = type.Name.ToLower();

            var files = new List<string>();
            // Find the "master" config file for the current config T

            foreach (var fileLowered in ConfigFileLoader.ConfigurationFilesLowered)
            {
                if (fileLowered.IsNot()) continue;

                if (!fileLowered.Contains(configNameLowered + ".")) continue;

                var values = fileLowered.Split('.');

                if (values != null && values.Length > 1 && values[^2].Contains(configNameLowered))
                {
                    if (values[^2].Contains("\\" + configNameLowered) ||
                        values[^2].Contains("/" + configNameLowered))
                    {
                        files.Add(fileLowered);
                    }
                }
            }

            // Add transformation file for environment if found
            if (ConfigFileLoader.environmentNameLowered.Is())
            {
                var configTransformationName = configNameLowered + "." + ConfigFileLoader.environmentNameLowered + ".";

                foreach (var fileLowered in ConfigFileLoader.ConfigurationFilesLowered)
                    if (fileLowered.Contains(configTransformationName) &&
                        !files.Contains(fileLowered))
                    {
                        files.Add(fileLowered);
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
