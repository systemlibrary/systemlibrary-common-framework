using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace SystemLibrary.Common.Framework;

partial class CryptationKey
{
    static string TryGetKeyFromAppName()
    {
        //// AddDataProtection() has not been invoked, returning fast
        var dataProtectionOptions = ServiceProviderInstance.Current.GetService<IOptions<DataProtectionOptions>>();
        if (dataProtectionOptions?.Value == null) return null;

        var appName = dataProtectionOptions.Value.ApplicationDiscriminator;

        return appName;
    }
}
