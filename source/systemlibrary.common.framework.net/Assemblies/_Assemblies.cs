using System.Diagnostics;

using Asm = System.Reflection.Assembly;

namespace SystemLibrary.Common.Framework;

public static partial class Assemblies
{
    static Assemblies()
    {
        var assembliesLoaded = AppDomain.CurrentDomain.GetAssemblies();

        var whiteListedAssemblies = new List<Asm>();

        foreach (var asm in assembliesLoaded)
        {
            if (!asm.FullName.StartsWithAny(BlacklistedAssemblyNames))
            {
                whiteListedAssemblies.Add(asm);
            }
            else if(!IsKestrelMainHostChecked)
            {
                if (asm.FullName.Contains("Kestrel"))
                {
                    IsKestrelMainHostChecked = true;

                    var tmp = false;

                    try
                    {
                        tmp = !Console.IsOutputRedirected && Console.OpenStandardInput(1) != Stream.Null;

                        if (tmp)
                        {
                            var processName = Process.GetCurrentProcess().ProcessName;

                            tmp = !processName.Contains("iis", StringComparison.OrdinalIgnoreCase);
                        }
                    }
                    catch
                    {
                        // Swallow
                    }
                    IsKestrelMainHost = tmp;
                }
            }
        }
        WhiteListedAssemblies = whiteListedAssemblies;
    }

    internal static bool IsKestrelMainHostChecked;

    internal static bool IsKestrelMainHost;
}