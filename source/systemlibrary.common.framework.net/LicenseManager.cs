namespace SystemLibrary.Common.Framework.Licensing;

internal class License
{
    static string LicenseKey = "Gm-09k20#gkm.!51";

    static bool? _IsValid;
    static object IsValidLock = new object();

    internal bool IsValid()
    {
        if (_IsValid != null) return _IsValid.Value;

        lock (IsValidLock)
        {
            if (_IsValid != null) return _IsValid.Value;

            _IsValid = IsValidToday();
        }

        if(!_IsValid.Value)
        {
            Log.Error("License missing or expired");
        }

        return _IsValid.Value;
    }

    static bool IsValidToday()
    {
        if (!EnvironmentConfig.IsProd) return true;

        var license = AppSettings.Current.SystemLibraryCommonFramework.License;

        if (license.IsNot()) return false;

        license = license.Decrypt(LicenseKey);

        var parts = license.Split('|');

        if (parts.Length != 3) return false;

        if (parts[2] != "SLF") return false;

        if (parts[0].IsNot()) return false;

        var created = parts[1].ToDateTime();

        var expires = created.AddYears(1);

        return expires > DateTime.Now;
    }

    internal string Generate(string companyName)
    {
        return (companyName + "|" + DateTime.Now + "|" + "SLF").Encrypt(LicenseKey);
    }
}
