using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;

namespace SystemLibrary.Common.Framework;

internal static class ServiceProviderInstance
{
    static IServiceProvider Local;

    internal static IServiceProvider Instance;

    static object LocalLock = new object();

    internal static IServiceProvider Current
    {
        get
        {
            if (Instance == null)
            {
                if (Local == null)
                {
                    lock (LocalLock)
                    {
                        if (Local != null) return Local;

                        // Auto-generating a built-in service provider for data protection and context access
                        // in 'unit test' scenarios and in 'console applications' if one does not register services oneself
                        var serviceCollection = new ServiceCollection();
                        serviceCollection
                            .AddDataProtection()
                            .SetApplicationName("app")
                            .SetDefaultKeyLifetime(TimeSpan.FromDays(365 * 100));

                        Local = serviceCollection.BuildServiceProvider();
                    }
                }
                return Local;
            }
            return Instance;
        }
    }
}
