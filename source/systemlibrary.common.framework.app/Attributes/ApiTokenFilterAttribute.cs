using Microsoft.AspNetCore.Mvc.Filters;

namespace SystemLibrary.Common.Framework.App;

/// <summary>
/// Filters access based on `api-token` header
/// <para>Optional: set a custom token header name</para>
/// <para>Input:</para>
/// `null`: Allows all
/// <para>`*`: Allows all</para>
/// - A regex: Value must satisfy the regex pattern
/// <para>- A pipe separated list: Any part is within the Value in a case insensitive check</para>
/// `text`: Value must match, case sensitive
/// <example>
/// <code>
/// [ApiTokenFilter(null)] // Allows all
/// [ApiTokenFilter("*")] // Allows all
/// [ApiTokenFilter("^[ab0-4]{4,}$")] // Allow any tokens containing `a, b, 0, 1, 2, 3 or 4` with a minimum length of 4
/// [ApiTokenFilter("SystemLibrary\.com")] // Exact match of SystemLibrary.com, case sensitive
/// [ApiTokenFilter("test.systemlibrary\.com|test2.systemlibrary.com")] // Allow tokens containing test.systemlibrary.com or test2.systemlibrary.com, case insensitive
/// </code>
/// </example>
/// </summary>
/// <remarks>
/// <para>Validating as a regex expression requires any of these characters: ^$*?[</para>
/// <para>Validate case insensitive with a string.Contains match requires at least one delimiter |</para>
/// <para>Falls back to normal string equals comparison, case sensitive</para>
/// </remarks>
public class ApiTokenFilterAttribute : BaseApiFilterAttribute
{
    new string Match;
    string HeaderName;

    /// <summary>
    /// match: either a regex, an exact string, or strings delimited by |
    /// </summary>
    public ApiTokenFilterAttribute(string match, string headerName = "api-token")
    {
        this.Match = match;
        this.HeaderName = headerName;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var value = context?.HttpContext?.Request?.Headers[this.HeaderName].ToString();

        try
        {
            var hasAccess = RequestHasValidHeaderValue(Match, value);

            if (hasAccess)
                base.OnActionExecuting(context);
            else
                base.OnAccessDenied(context, this.HeaderName + " is incorrect");
        }
        catch(Exception ex)
        {
            base.OnAccessDenied(context, this.HeaderName + " threw: " + ex.Message);
        }
    }
}
