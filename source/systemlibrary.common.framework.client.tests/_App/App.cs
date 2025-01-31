using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using SystemLibrary.Common.Framework.App.Extensions;

namespace SystemLibrary.Common.Framework.App.Tests;

public static class App
{
    public static void Start<TInterface, TImplementation>(string environment = "local")
        where TInterface : class
        where TImplementation : class, TInterface
    {
        var appSettingsPath = AppContext.BaseDirectory + "\\appSettings.json";

        var app = Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(config =>
            {
                config.UseEnvironment(environment);
                config.UseStartup<Startup<TInterface, TImplementation>>();
            })
            .ConfigureAppConfiguration(config => config.AddJsonFile(appSettingsPath))
            .Build();

        app.Start();
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
            services.AddFrameworkServices();
            services.AddTransient<TInterface, TImplementation>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseFrameworkApp(env);
        }
    }
}
