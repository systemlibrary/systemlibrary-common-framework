using Microsoft.AspNetCore.DataProtection;

namespace SystemLibrary.Common.Framework;

internal static class KeyRingProtectorInstance
{
    static IDataProtector _KeyRingProtector;

    internal static IDataProtector Current
    {
        get
        {
            if (_KeyRingProtector == null)
            {
                var dataProtectionProvider = ServiceProviderInstance.Current.GetDataProtectionProvider();

                _KeyRingProtector = dataProtectionProvider.CreateProtector(AppInstance.AppName);
            }

            return _KeyRingProtector;
        }
    }
}
