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

                    bool IsWithinBin(string dir)
                    {
                        if (dir.Contains(".Tests\\", StringComparison.OrdinalIgnoreCase) || dir.Contains(".Test\\", StringComparison.OrdinalIgnoreCase))
                            return false;

                        if (dir.Contains("\\Tests\\", StringComparison.OrdinalIgnoreCase) || dir.Contains("\\Test\\", StringComparison.OrdinalIgnoreCase))
                            return false;

                        return dir.Contains("\\bin\\", StringComparison.OrdinalIgnoreCase) ||
                            dir.Contains("/bin/", StringComparison.OrdinalIgnoreCase) ||
                            dir.EndsWith("/bin", StringComparison.OrdinalIgnoreCase) ||
                            dir.EndsWith("\\bin", StringComparison.OrdinalIgnoreCase);
                    }

                    var temp = _ContentRootPath;
                    var i = 8;

                    while (IsWithinBin(temp))
                    {
                        i--;
                        if (i < 0) break;

                        temp = new DirectoryInfo(temp).Parent?.FullName;

                        if (temp.IsNot())
                            break;
                    }

                    var wasWithinBin = temp != _ContentRootPath;
                    if (wasWithinBin)
                    {
                        var files = Directory.GetFiles(temp, "*.csproj", SearchOption.TopDirectoryOnly);

                        if (files?.Length == 1)
                        {
                            _ContentRootPath = temp;
                        }
                        else
                        {
                            var appsettingFiles = Directory.GetFiles(temp, "appsettings.json", SearchOption.TopDirectoryOnly);
                            if (files?.Length >= 1)
                            {
                                _ContentRootPath = temp;
                            }
                        }
                    }

                    _ContentRootPath = _ContentRootPath.Replace("\\", "/");

                    if (_ContentRootPath.EndsWith("/", StringComparison.Ordinal))
                        _ContentRootPath = _ContentRootPath.Substring(0, _ContentRootPath.Length - 1);
                }
            }

            return _ContentRootPath;
        }
    }
}
