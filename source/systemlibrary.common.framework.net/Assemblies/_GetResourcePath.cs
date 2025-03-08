using System.Collections.Concurrent;
using System.Reflection;

namespace SystemLibrary.Common.Framework;

partial class Assemblies
{
    static ConcurrentDictionary<string, string[]> EmbeddedResourceNamesCache = new ConcurrentDictionary<string, string[]>();

    static string GetManifestResourceName(string relativeName, Assembly asm)
    {
        string[] embeddedResourceNames = null;
        if (EmbeddedResourceNamesCache.ContainsKey(asm.FullName))
        {
            embeddedResourceNames = EmbeddedResourceNamesCache[asm.FullName];
        }
        else
        {
            embeddedResourceNames = asm.GetManifestResourceNames();

            if (embeddedResourceNames == null) return null;

            EmbeddedResourceNamesCache.TryAdd(asm.FullName, embeddedResourceNames);
        }

        if (relativeName.StartsWith(".."))
            relativeName = relativeName.Substring(2);

        if (relativeName.StartsWith("."))
            relativeName = relativeName.Substring(1);

        if (relativeName.StartsWith("~"))
            relativeName = relativeName.Substring(1);

        var dottedEmbeddedResourceName = relativeName.ReplaceAllWith(".", "\\", "/");

        foreach (var resource in embeddedResourceNames)
        {
            if (resource.EndsWith(dottedEmbeddedResourceName, StringComparison.Ordinal))
                return resource;
        }

        foreach (var resource in embeddedResourceNames)
        {
            if (resource.EndsWith(dottedEmbeddedResourceName, StringComparison.OrdinalIgnoreCase))
                return resource;
        }

        return null;
    }
}