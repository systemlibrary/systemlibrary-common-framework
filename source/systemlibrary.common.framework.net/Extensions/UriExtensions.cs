using System;

namespace SystemLibrary.Common.Framework.Extensions;

/// <summary>
/// This class contains extension methods for Uri
/// 
/// <para>For instance: GetPrimaryDomain(), etc...</para>
/// </summary>
public static class UriExtensions
{
    /// <summary>
    /// Returns the domain part of the uri or blank, never null:
    /// <para>https://www.sub1.sub2.domain.com => domain.com</para>
    /// </summary>
    /// <example>
    /// <code class="language-csharp hljs">
    /// var result = new Uri('https://systemlibrary.com/image?q=90&amp;format=jpg').GetPrimaryDomain();
    /// // result is "systemlibrary.com"
    /// 
    /// var result = new Uri('https://systemlibrary.github.io/systemlibrary-common-framework/image?q=90&amp;format=jpg').GetPrimaryDomain();
    /// // result is "github.io"
    /// </code>
    /// </example>
    /// <returns>Primary domain name or "", never null</returns>
    public static string GetPrimaryDomain(this Uri uri, string topLevelDomain = ".com")
    {
        if (uri == null)
            return "";

        string host;
        if (uri.IsAbsoluteUri)
            host = uri.Host;
        else
            host = uri.OriginalString;

        if (host.IsNot())
            return "";

        if (!host.Contains("."))
            return host + topLevelDomain;

        var values = host.Split('.');

        var length = values.Length;

        if (length == 1)
            return host;

        if (length == 2)
        {
            if (values[1].Length <= 4)
                return host;
        }

        if (values[length - 1].Length > 4)
            return values[length - 1] + topLevelDomain;

        return values[length - 2] + "." + values[length - 1];
    }
}
