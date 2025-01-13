using Microsoft.Extensions.DependencyInjection;

namespace SystemLibrary.Common.Framework.App;

/// <summary>
/// Services is a global way to reuse the configured service provider and collection
/// </summary>
public static class Services
{
    /// <summary>
    /// Get service as T or default if not found
    /// </summary>
    public static T Get<T>() where T : class
    {
        return ServiceProviderInstance.Current.GetService<T>();
    }
}