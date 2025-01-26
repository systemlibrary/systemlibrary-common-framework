using Microsoft.AspNetCore.Mvc.Filters;

using SystemLibrary.Common.Framework.App.Extensions;

namespace SystemLibrary.Common.Framework.App;

/// <summary>
/// Filters access based on `User-Agent` header
/// <para>NOTE: If attribute is added it blocks most known spiders, crawlers and bots even if you allow all</para>
/// <para>Input:</para>
/// `null`: Allows all
/// <para>`*`: Allows all</para>
/// - A regex: Value must satisfy the regex pattern
/// <para>- A pipe separated list: Any part is within the Value in a case insensitive check</para>
/// `text`: Value must match, case sensitive
/// <example>
/// <code>
/// [UserAgentFilter(null)] // Allows all
/// [UserAgentFilter("*")] // Allows all
/// [UserAgentFilter("^[ab0-4]{4,}$")] // Allow user agents containing only `a, b, 0, 1, 2, 3 or 4` with a minimum length of 4
/// [UserAgentFilter("User Agent")] // Exact match of 'User Agent', case sensitive
/// [UserAgentFilter("firefox|edg|chrome")] // Allow user agents containing firefox or edg or chrome, case insensitive
/// </code>
/// </example>
/// </summary>
/// <remarks>
/// <para>Validating as a regex expression requires any of these characters: ^$*?[</para>
/// <para>Validate case insensitive with a string.Contains match requires at least one delimiter |</para>
/// <para>Falls back to normal string equals comparison, case sensitive</para>
/// </remarks>
public class UserAgentFilterAttribute : BaseApiFilterAttribute
{
    string Match;

    static HashSet<string> BlacklistedUserAgents = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        // Common Search Engine Bots (to prevent indexing of your API)
        "Googlebot", "Bingbot", "Baiduspider", "YandexBot", "DuckDuckBot",
        "AhrefsBot", "SemrushBot", "MJ12bot", "Applebot", "Sogou",
        "Exabot", "facebookexternalhit", "Twitterbot",

        // Common Automation Scripts (used for scraping and attacks)
        "Java/", "Scrapy", "Lynx/",

        // Generic bots and scrapers
        "Bot/", "crawler/", "spider/"
    };

    /// <summary>
    /// match: either a regex, an exact string, or strings delimited by |
    /// </summary>
    public UserAgentFilterAttribute(string match)
    {
        this.Match = match;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var value = context?.HttpContext?.Request.UserAgent();

        var hasAccess = value == null || !IsBlacklisted(value);

        if (hasAccess)
            hasAccess = RequestHasValidHeaderValue(Match, value);

        // Debug.Log("FilterAttribute value: " + value + " = " + hasAccess);

        if (hasAccess)
        {
            base.OnActionExecuting(context);
        }
        else
        {
            base.OnAccessDenied(context, "User agent is incorrect");
        }
    }

    static bool IsBlacklisted(string value)
    {
        foreach (var blockedAgent in BlacklistedUserAgents)
        {
            if (value.Contains(blockedAgent, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        return false;
    }
}
