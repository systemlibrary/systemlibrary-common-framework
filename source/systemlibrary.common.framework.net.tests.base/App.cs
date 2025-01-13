using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SystemLibrary.Common.Framework.Net.Tests;

public static class App
{
    public static IServiceProvider Start<TInterface, TImplementation>(string environment = "local")
        where TInterface : class
        where TImplementation : class, TInterface
    {
        var appSettingsPath = AppContext.BaseDirectory + "\\Configs\\AppSettings\\appSettings.json";

        var app = Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(config =>
            {
                config.UseEnvironment(environment);
                config.UseStartup<Startup<TInterface, TImplementation>>();
            })
            .ConfigureAppConfiguration(config => config.AddJsonFile(appSettingsPath))
            .Build();

        app.Start();

        return app.Services;
    }

    class Startup<TInterface, TImplementation>
            where TInterface : class
            where TImplementation : class, TInterface
    {
        public Startup()
        {
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<TInterface, TImplementation>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
        }
    }
}
