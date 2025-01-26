using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace SystemLibrary.Common.Framework.App.Extensions;

internal static class IServiceProviderExtensions
{
    internal static IServiceProvider UseServiceProvider(this IServiceProvider serviceProvider)
    {
        HttpContextInstance.HttpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();

        ActionContextInstance.ActionContextAccessor = serviceProvider.GetRequiredService<IActionContextAccessor>();

        ServiceProviderInstance.Instance = serviceProvider;

        return serviceProvider;
    }
}
