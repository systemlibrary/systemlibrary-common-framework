using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace SystemLibrary.Common.Framework;

public static class FrameworkAppBuilder
{
    /// <summary>
    /// Create a HostBuilder for a specific HostType, with configuration files and environment also configured
    /// </summary>
    /// <typeparam name="T">Startup class that contains methods for Service and Middleware registrations</typeparam>
    /// <param name="args"></param>
    /// <param name="type">Usually IIS or Kestrel in a web application context</param>
    /// <param name="frameworkKeyDirectory">Set directory of where the framework can find a 'key.xml'-file
    /// <para>Directory should be outside of app root for security reasons</para>
    /// <para>If a Key file was found in the Dir, it will be used whenever you invoke the extension methods without arguments: Encrypt() and Decrypt()</para>
    /// <para>If not set, you can set the AppName in appSettings or when calling AddDataProtection to be the Key used when you invoke Encrypt() and Decrypt()</para>
    /// If AppName is not set key defaults to ABCD
    /// </param>
    /// <param name="appSettingsFullPath">Defaults to BaseDirectory/appSettings.json</param>
    /// <param name="reloadOnConfigChange"></param>
    /// <returns>Returns IHostBuilder</returns>
    public static IHostBuilder Create<T>(
     string[] args,
     HostType type,
     string frameworkKeyDirectory = null,
     string appSettingsFullPath = null,
     bool reloadOnConfigChange = false
 ) where T : class
    {
        appSettingsFullPath ??= Path.Combine(AppContext.BaseDirectory, "appSettings.json");

        if (!appSettingsFullPath.EndsWith("appSettings.json"))
            throw new Exception($"{appSettingsFullPath} must end with appSettings.json");

        CryptationKey.FrameworkKeyDirectory = frameworkKeyDirectory.ToOsFriendlyPath();

        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? EnvironmentConfig.Current.Name;

        var hostBuilder = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(config =>
            {
                AddJsonConfigurations(config, appSettingsFullPath, environment, reloadOnConfigChange);
            });

        ConfigureWebOrGenericHost<T>(hostBuilder, type, environment);

        return hostBuilder;
    }

    static void AddJsonConfigurations(
        IConfigurationBuilder config,
        string appSettingsFullPath,
        string environment,
        bool reloadOnChange)
    {
        config.AddJsonFile(appSettingsFullPath);

        if (environment.Is())
            config.AddJsonFile(appSettingsFullPath.Replace(".json", $".{environment}.json"), true, reloadOnChange);
    }

    static void ConfigureWebOrGenericHost<T>(IHostBuilder hostBuilder, HostType type, string environment) where T : class
    {
        if (type == HostType.Kestrel)
        {
            hostBuilder.ConfigureWebHost(webBuilder =>
            {
                if (environment.Is())
                    webBuilder.UseEnvironment(environment);

                webBuilder.UseKestrel(options =>
                {
                    options.ListenAnyIP(50051);
                });
                webBuilder.UseStartup<T>();
            });
        }
        else if (type == HostType.IIS)
        {
            hostBuilder.ConfigureWebHostDefaults(webBuilder =>
            {
                if (environment.Is())
                    webBuilder.UseEnvironment(environment);
                webBuilder.UseStartup<T>();
            });
        }
        else
        {
            hostBuilder.ConfigureServices((_, services) =>
            {
            });
        }
    }

    public enum HostType
    {
        /// <summary>
        /// IIS or IIS Express
        /// </summary>
        IIS,
        /// <summary>
        /// Kestrel
        /// </summary>
        Kestrel,
        /// <summary>
        /// Unknown returns a default app builder only
        /// </summary>
        Unknown = 9999
    }
}
