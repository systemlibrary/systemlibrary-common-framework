using Microsoft.AspNetCore.Mvc.Filters;

namespace SystemLibrary.Common.Framework.App;

/// <summary>
/// Filters access based on `Origin` header
/// <para>Input:</para>
/// `null`: Allows all
/// <para>`*`: Allows all</para>
/// - A regex: Value must satisfy the regex pattern
/// <para>- A pipe separated list: Any part is within the Value in a case insensitive check</para>
/// `text`: Value must match, case sensitive
/// <example>
/// <code>
/// [OriginFilter(null)] // Allows all
/// [OriginFilter("*")] // Allows all
/// [OriginFilter("^[ab0-4]{4,}$")] // Allow origin containing `a, b, 0, 1, 2, 3 or 4` with a minimum length of 4
/// [OriginFilter("SystemLibrary\.com")] // Exact match of SystemLibrary.com, case sensitive
/// [OriginFilter("test.systemlibrary\.com|test2.systemlibrary.com")] // Allow origins containing test.systemlibrary.com or test2.systemlibrary.com, case insensitive
/// </code>
/// </example>
/// </summary>
/// <remarks>
/// <para>Validating as a regex expression requires any of these characters: ^$*?[</para>
/// <para>Validate case insensitive with a string.Contains match requires at least one delimiter |</para>
/// <para>Falls back to normal string equals comparison, case sensitive</para>
/// </remarks>
public class OriginFilterAttribute : BaseApiFilterAttribute
{
    new string Match;

    /// <summary>
    /// match: either a regex, an exact string, or strings delimited by |
    /// </summary>
    public OriginFilterAttribute(string match = null)
    {
        this.Match = match;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var value = context?.HttpContext?.Request?.Headers["Origin"].ToString();

        var hasAccess = RequestHasValidHeaderValue(this.Match, value);

        if (hasAccess)
        {
            base.OnActionExecuting(context);
        }
        else
        {
            base.OnAccessDenied(context, "Origin is missing or incorrect");
        }
    }
}