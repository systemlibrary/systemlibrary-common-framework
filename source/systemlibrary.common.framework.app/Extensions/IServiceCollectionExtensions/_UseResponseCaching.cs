using Microsoft.Extensions.DependencyInjection;

namespace SystemLibrary.Common.Framework.App.Extensions;

static partial class IServiceCollectionExtensions
{
    static IServiceCollection UseResponseCaching(this IServiceCollection services, FrameworkOptions options)
    {
        if (options.UseResponseCaching)
        {
            services = services.AddResponseCaching(opt2 =>
            {
                opt2.SizeLimit = 2500L * 1024 * 1024;       //2.5GB
                opt2.MaximumBodySize = 8 * 1024 * 1024;     //8MB
                opt2.UseCaseSensitivePaths = false;
            });
        }

        return services;
    }
}