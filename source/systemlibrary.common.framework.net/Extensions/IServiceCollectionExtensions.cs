﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace SystemLibrary.Common.Framework.Extensions;

internal static class IServiceCollectionExtensions
{
    internal static IServiceCollection AddCommonServices<TLogWriter>(this IServiceCollection serviceCollection) where TLogWriter : class, ILogWriter
    {
        serviceCollection.AddScoped<ILogWriter, TLogWriter>();

        return serviceCollection.AddCommonServices();
    }

    internal static IServiceCollection AddCommonServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        serviceCollection.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();

        // Temporarily building service provider 'at the start' which contains our LogWriter and these Context Accessors only
        var tempProvider = serviceCollection.BuildServiceProvider();

        ServiceProviderInstance.Instance = tempProvider;

        return serviceCollection;
    }
}
