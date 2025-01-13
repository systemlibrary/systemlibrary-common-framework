using Microsoft.Extensions.DependencyInjection;

namespace SystemLibrary.Common.Framework.App.Extensions;

static partial class IServiceCollectionExtensions
{
    static IMvcBuilder UseAddRazorPages(this IServiceCollection services, FrameworkServicesOptions options = null, IMvcBuilder builder = null)
    {
        if (builder == null)
            builder = services.AddRazorPages();

        return builder;
    }
}