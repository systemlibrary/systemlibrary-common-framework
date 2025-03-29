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

    /// <summary>
    /// Returns the content root path without ending slash
    /// <para>Usually the parent of the 'bin' folder, where appsettings.json resides</para>
    /// <para>Exception 1: if the content root path found contains .test(s) or \test(s)\ it is returned as is, so Test projects can run from the output compiled folder</para>
    /// <para>Exception 2: if the content root path contains \lib\ it is returned as is, supporting multiple project builds into the same lib folder</para>
    /// </summary>
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

                    bool IsInBinOrLib(string dir)
                    {
                        if (dir.Contains(".Tests\\", StringComparison.OrdinalIgnoreCase) || dir.Contains(".Test\\", StringComparison.OrdinalIgnoreCase))
                            return false;

                        if (dir.Contains("\\Tests\\", StringComparison.OrdinalIgnoreCase) || dir.Contains("\\Test\\", StringComparison.OrdinalIgnoreCase))
                            return false;

                        if (dir.Contains("\\lib\\", StringComparison.OrdinalIgnoreCase))
                            return false;

                        return dir.Contains("\\bin\\", StringComparison.OrdinalIgnoreCase) ||
                            dir.Contains("/bin/", StringComparison.OrdinalIgnoreCase) ||
                            dir.EndsWith("/bin", StringComparison.OrdinalIgnoreCase) ||
                            dir.EndsWith("\\bin", StringComparison.OrdinalIgnoreCase);
                    }

                    var temp = _ContentRootPath;
                    var i = 8;

                    while (IsInBinOrLib(temp))
                    {
                        i--;
                        if (i < 0) break;

                        temp = new DirectoryInfo(temp).Parent?.FullName;

                        if (temp.IsNot())
                            break;
                    }

                    var wasWithinBin = temp.Is() && temp != _ContentRootPath;
                    if (wasWithinBin)
                    {
                        _ContentRootPath = temp;
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
