using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SystemLibrary.Common.Framework.Tests;

public static class FrameworkApp
{
    public static IHost Start(string environment = "LOCAL", bool stop = true)
    {
        var appSettingsPath = AppContext.BaseDirectory + "\\Configs\\AppSettings\\appSettings.json";

        var frameworkApp = FrameworkAppBuilder.Create<Startup>(
            null,
            FrameworkAppBuilder.HostType.Kestrel,
            @"C:\temp\",
            appSettingsPath)
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
                logging.SetMinimumLevel(LogLevel.Warning);
            })
            .Build();

        frameworkApp.Start();
        if(stop)
            frameworkApp.StopAsync(new CancellationToken(true)).Wait();

        return frameworkApp;
    }
}
