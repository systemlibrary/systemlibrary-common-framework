using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

using SystemLibrary.Common.Framework.App.Extensions;

namespace SystemLibrary.Common.Framework.Tests;

internal class Startup
{
    public static FrameworkServicesOptions ServiceOptions;
    public static FrameworkAppOptions AppOptions;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddFrameworkServices<LogWriter>(ServiceOptions);
        ServiceOptions = null;
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseFrameworkMiddlewares(env, AppOptions);
        AppOptions = null;
    }
}
