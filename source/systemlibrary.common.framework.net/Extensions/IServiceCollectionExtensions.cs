using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace SystemLibrary.Common.Framework.Extensions;

internal static class IServiceCollectionExtensions
{
    internal static IServiceCollection AddCommonServices(this IServiceCollection serviceCollection)
    {
        // Inserting a default instance, should override the default System.Text.Json serialization that occurs whenever someone uses serialization/deserialization outside .Json() extensions
        serviceCollection.TryAddSingleton(JsonSerializerOptionsInstance.Current);

        serviceCollection.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        serviceCollection.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();

        return serviceCollection;
    }
}
