using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using SystemLibrary.Common.Framework.App;

namespace SystemLibrary.Common.Framework.Extensions;

static partial class IServiceCollectionextensions
{
    internal static IServiceCollection AddCommonServices<TLogWriter>(this IServiceCollection serviceCollection) where TLogWriter : class, ILogWriter
    {
        serviceCollection.AddScoped<ILogWriter, TLogWriter>();

        serviceCollection.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        serviceCollection.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();

        // NOTe: Temporarily building the service provider early on to be able to use LogWriter and the Context instances throughout the registration
        var tempProvider = serviceCollection.BuildServiceProvider();

        ServiceProviderInstance.Instance = tempProvider;
        
        HttpContextInstance.HttpContextAccessor = tempProvider.GetRequiredService<IHttpContextAccessor>();

        ActionContextInstance.ActionContextAccessor = tempProvider.GetRequiredService<IActionContextAccessor>();

        return serviceCollection;
    }
}
