using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace SystemLibrary.Common.Framework;

internal static class AppInstance
{
    static string _AppName;

    internal static string AppName
    {
        get
        {
            if (_AppName == null)
            {
                _AppName = ServiceProviderInstance.Current.GetService<IOptions<DataProtectionOptions>>()
                    ?.Value?.ApplicationDiscriminator
                    ?? AppSettings.Current.DataProtection.AppName;
            }
            return _AppName;
        }
    }

    static string _AspNetCoreEnvironment;

    internal static string AspNetCoreEnvironment
    {
        get
        {
            if (_AspNetCoreEnvironment == null)
            {
                try
                {
                    _AspNetCoreEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                }
                catch (Exception ex)
                {
                    Debug.Log("[AppInstance] ASPNETCORE_ENVIRONMENT could not be read " + ex.Message);
                }

                if (_AspNetCoreEnvironment.IsNot())
                {
                    var args = Environment.GetCommandLineArgs();
                    _AspNetCoreEnvironment = args?
                        .FirstOrDefault(a => a.StartsWith("--environment ", StringComparison.OrdinalIgnoreCase))?
                        .Split(' ')[1];

                    if (_AspNetCoreEnvironment.IsNot())
                    {
                        _AspNetCoreEnvironment = args?
                            .FirstOrDefault(a => a.StartsWith("--e ", StringComparison.OrdinalIgnoreCase))?
                            .Split(' ')[1];
                    }
                    if (_AspNetCoreEnvironment.IsNot())
                    {
                        _AspNetCoreEnvironment = args?
                            .FirstOrDefault(a => a.StartsWith("--environment=", StringComparison.OrdinalIgnoreCase))?
                            .Split('=')[1];
                    }
                }

                _AspNetCoreEnvironment ??= "";
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

                        if (_ContentRootPath.Contains("\\Tests\\", StringComparison.OrdinalIgnoreCase) || _ContentRootPath.Contains("\\Test\\", StringComparison.OrdinalIgnoreCase))
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
