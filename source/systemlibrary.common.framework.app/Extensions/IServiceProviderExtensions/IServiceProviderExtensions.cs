using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace SystemLibrary.Common.Framework.App.Extensions;

public static class IServiceProviderExtensions
{
    /// <summary>
    /// Pass a reference of the Service Provider which will be used to loook up services from
    /// </summary>
    public static IServiceProvider UseFrameworkServiceProvider(this IServiceProvider serviceProvider)
    {
        HttpContextInstance.HttpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();

        ActionContextInstance.ActionContextAccessor = serviceProvider.GetRequiredService<IActionContextAccessor>();

        return serviceProvider;
    }
}
