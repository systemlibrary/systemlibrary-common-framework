using System.Reflection;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace SystemLibrary.Common.Framework;

internal static class AppInstance
{
    static string _AppName;

    // NOTE: An auto built-in app name, do not confuse it with "SetApplicationName()" method during "AddDataProtection" services
    internal static string AppName
    {
        get
        {
            if (_AppName == null)
            {
                var dataProtectionOptions = ServiceProviderInstance.Current.GetService<IOptions<DataProtectionOptions>>();

                var dataProtectionAppName = dataProtectionOptions?.Value?.ApplicationDiscriminator;

                if (dataProtectionAppName.Is())
                {
                    _AppName = dataProtectionAppName;
                }
                else
                {
                    var appNameFromAppSettings = AppSettings.Current.DataProtection.AppName;
                    if (appNameFromAppSettings.Is())
                    {
                        _AppName = appNameFromAppSettings;
                    }
                    else
                    {
                        _AppName = "app" +
                             Wash(Assembly.GetEntryAssembly()?.GetName()?.Name, 16) +
                             Wash(Assembly.GetExecutingAssembly()?.GetName()?.Name, 8);
                    }
                }
            }
            return _AppName;
        }
    }

    static string Wash(string name, int maxLength)
    {
        if (name == null) return "";

        var washed = new string(name.Where(char.IsLetter).ToArray());

        return washed?
            .ToLower()?
            .MaxLength(maxLength);
    }

    static string _AspNetCoreEnvironment;

    internal static string AspNetCoreEnvironment
    {
        get
        {
            if (_AspNetCoreEnvironment == null)
            {
                _AspNetCoreEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                _AspNetCoreEnvironment ??= "";

                //Debug.Log("ASPNETCORE_ENVIRONMENT is " + _AspNetCoreEnvironment);
            }

            return _AspNetCoreEnvironment;
        }
    }

    static string _ContentRootPath;
    static object _ContentRootPathLock = new object();

    // Does not end with slash
    // Cannot be inside 'bin', if so, the parent of 'bin' is returned, exception is if project name ends in .Test(s)</para>
    internal static string ContentRootPath
    {
        get
        {
            if (_ContentRootPath == null)
            {
                lock (_ContentRootPathLock)
                {
                    if (_ContentRootPath != null) return _ContentRootPath;

                    try
                    {
                        _ContentRootPath = AppDomain.CurrentDomain?.GetData("ContentRootPath") + "";
                    }
                    catch
                    {
                        // swallow
                    }

                    if (_ContentRootPath.IsNot())
                        _ContentRootPath = new DirectoryInfo(AppContext.BaseDirectory).FullName;


                    bool IsWithinBin()
                    {
                        if (_ContentRootPath.Contains(".Tests\\", StringComparison.OrdinalIgnoreCase) || _ContentRootPath.Contains(".Test\\", StringComparison.OrdinalIgnoreCase))
                            return false;

                        return _ContentRootPath.Contains("\\bin\\", StringComparison.OrdinalIgnoreCase) ||
                            _ContentRootPath.Contains("/bin/", StringComparison.OrdinalIgnoreCase) ||
                            _ContentRootPath.EndsWith("/bin", StringComparison.OrdinalIgnoreCase) ||
                            _ContentRootPath.EndsWith("\\bin", StringComparison.OrdinalIgnoreCase);
                    }

                    while (IsWithinBin())
                    {
                        var temp = _ContentRootPath;

                        _ContentRootPath = new DirectoryInfo(_ContentRootPath).Parent?.FullName;

                        if (_ContentRootPath == null)
                        {
                            _ContentRootPath = temp;
                            break;
                        }
                    }

                    if (_ContentRootPath.EndsWith("\\", StringComparison.Ordinal))
                        _ContentRootPath = _ContentRootPath.Substring(0, _ContentRootPath.Length - 1);

                    if (_ContentRootPath.EndsWith("/", StringComparison.Ordinal))
                        _ContentRootPath = _ContentRootPath.Substring(0, _ContentRootPath.Length - 1);

                    _ContentRootPath = _ContentRootPath.Replace("\\", "/");
                }
            }

            return _ContentRootPath;
        }
    }
}
