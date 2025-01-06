using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace SystemLibrary.Common.Framework;

partial class CryptationKey
{
    static string TryGetKeyFromAppNameOrAsmName()
    {
        //// DataProtection is not added at all; we do not enforce a "Custom Key" based on "AppName"
        var keyManagementOptions = ServiceProviderInstance.Current.GetService<IOptions<KeyManagementOptions>>();
        if (keyManagementOptions?.Value == null) return null;

        var dataProtectionOptions = ServiceProviderInstance.Current.GetService<IOptions<DataProtectionOptions>>();
        
        var key = dataProtectionOptions?.Value?.ApplicationDiscriminator;

        // KeyManagementOptions was specified, but without a 'data protection app name', using custom appName as Key
        if (key.IsNot())
        {
            Debug.Log("DataProtection service is added, but without AppName, creating one based on assembly info");

            key = AppInstance.AppName;

            dataProtectionOptions.Value.ApplicationDiscriminator = key;
        }

        return key;
    }
}
