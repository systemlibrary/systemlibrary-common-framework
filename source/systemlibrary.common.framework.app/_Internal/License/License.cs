using System.Runtime.InteropServices;

using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework.Licensing;

internal static class License
{
    internal enum Tier
    {
        Bronze,
        Silver,
        Gold
    }

    [DllImport("SystemLibrary.Common.Framework.LicenseEncKey.dll", CallingConvention = CallingConvention.StdCall)]
    public static extern IntPtr LicenseEncKey();

    static string _GetLicenseEncKey;
    static string GetLicenseEncKey
    {
        get
        {
            if (_GetLicenseEncKey == null)
                _GetLicenseEncKey = Marshal.PtrToStringAnsi(LicenseEncKey());

            return _GetLicenseEncKey;
        }
    }

    static Dictionary<Tier, bool> TiersLicensed = new Dictionary<Tier, bool>();

    static object _lock = new object();

    internal static bool BypassEnvironmentCheck;
    internal static string TestLicense;

    internal static bool Gold()
    {
        return IsTierValid(Tier.Gold);
    }

    internal static bool Silver()
    {
        return IsTierValid(Tier.Silver);
    }

    internal static bool Bronze()
    {
        return IsTierValid(Tier.Bronze);
    }

    internal static string Generate(string companyId, string companyName, Tier tier)
    {
        companyId = companyId?.Replace("|", "#").MaxLength(32);
        companyName = companyName?.Replace("|", "#").MaxLength(32);

        return (companyId + "|" + companyName + "|" + tier + "|" + DateTime.Now.ToString("yyyy-MM-dd")).Encrypt(Marshal.PtrToStringAnsi(LicenseEncKey()));
    }

    static bool IsTierValid(Tier tier)
    {
        if (TiersLicensed.ContainsKey(tier) && TestLicense == null)
            return TiersLicensed[tier];

        Debug.Log("[License] Check license tier " + tier);

        lock (_lock)
        {
            if (TiersLicensed.TryGetValue(tier, out bool isValid2) && TestLicense == null) return isValid2;

            TiersLicensed[tier] = GetTierState(tier);
        }

        Debug.Log("[License] Licenseed: " + TiersLicensed[tier]);

        return TiersLicensed[tier];
    }

    static bool GetTierState(Tier licenseTier)
    {
        if (EnvironmentConfig.IsLocal)
        {
            if (!BypassEnvironmentCheck) return true;
        }
        var license = TestLicense ?? AppSettings.Current.SystemLibraryCommonFramework.License;

        if (license.IsNot()) return false;
        
        try
        {
            license = license.Decrypt(GetLicenseEncKey);
        }
        catch
        {
            return Invalid("invalid");
        }

        var parts = license.Split('|');

        if (parts.Length != 4) return Invalid("invalid length");

        var id = parts[0];
        var name = parts[1];
        var tier = parts[2];
        var created = parts[3];

        if (name == "SystemLibrary") return true;

        if (id.IsNot())
        {
            return Invalid("invalid company id");
        }

        if (name.IsNot())
        {
            return Invalid("invalid company name");
        }

        var tiers = EnumExtensions<Tier>.GetEnums();

        var isTierLowerOrEqual = false;

        foreach (var tierModel in tiers)
        {
            if (licenseTier == tierModel)
            {
                isTierLowerOrEqual = true;
                break;
            }
            if (tier.Equals(tierModel.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                break;
            }
        }

        if (!isTierLowerOrEqual)
            return Invalid("invalid plan");

        // No date, means no expiry and previous parts have matched, we return true
        if (created.IsNot()) return true;

        var expires = created.ToDateTime().AddYears(1);

        var hasNotExpired = expires > DateTime.Now;

        if (!hasNotExpired)
            return Invalid("expired");

        return hasNotExpired;
    }

    static bool Invalid(string message)
    {
        Log.Error("[License] " + message);

        return false;
    }
}
