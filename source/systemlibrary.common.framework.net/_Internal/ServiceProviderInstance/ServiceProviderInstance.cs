﻿using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;

using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework;

internal static class ServiceProviderInstance
{
    static IServiceProvider Local;

    internal static IServiceProvider Instance;

    internal static IServiceProvider Current
    {
        get
        {
            if (Instance == null)
            {
                if (Local == null)
                {
                    // Auto-generating a built-in service provider for data protection and context access
                    // in 'unit test' scenarios and in 'console applications' if one does not register services oneself
                    var serviceCollection = new ServiceCollection();

                    serviceCollection
                        .AddCommonServices()
                        .AddDataProtection()
                        .SetDefaultKeyLifetime(TimeSpan.FromDays(365 * 100));

                    Local = serviceCollection.BuildServiceProvider();
                }
                return Local;
            }
            return Instance;
        }
    }
}