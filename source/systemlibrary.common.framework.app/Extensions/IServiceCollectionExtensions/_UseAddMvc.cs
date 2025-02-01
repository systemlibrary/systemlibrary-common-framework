using Microsoft.Extensions.DependencyInjection;

namespace SystemLibrary.Common.Framework.App.Extensions;

partial class IServiceCollectionExtensions
{
    static IMvcBuilder UseAddMvc(this IServiceCollection services, FrameworkServiceOptions options)
    {
        var builder = services.AddMvc();

        builder.Services.Configure(ConfigureSupportedMediaTypes(options));

        return builder;
    }
}