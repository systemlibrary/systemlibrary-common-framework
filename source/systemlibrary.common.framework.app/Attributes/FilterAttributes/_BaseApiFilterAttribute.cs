using System.Collections.Concurrent;
using System.Text.RegularExpressions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework.App;

public class BaseApiFilterAttribute : ActionFilterAttribute
{
    static readonly ConcurrentDictionary<string, Regex> RegexCache = new ConcurrentDictionary<string, Regex>();

    const string RegexCharacters = @"^$*?[";
    static int RegexL = RegexCharacters.Length;

    static bool IsRegex(string match)
    {
        for (int i = 0; i < RegexL; i++)
            if (match.Contains(RegexCharacters[i])) return true;

        return match.Contains("/i");
    }

    protected bool RequestHasValidHeaderValue(string match, string value)
    {
        if (match == null) return true;

        if (match == "*") return true;

        if (value == null) return false;

        if (IsRegex(match))
        {
            var regex = RegexCache.Cache(match, () =>
            {
                return new Regex(match, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            });

            var regexMatch = regex.IsMatch(value);

            if (regexMatch) return true;
        }

        if (match.Contains("|"))
        {
            var parts = match.Split('|');

            foreach (var part in parts)
            {
                var p = part?.Trim();

                if (p == null) return false;

                if (p == "" && value == "") return true;

                if (p.Length == 0) return false;

                // Contains as this check is reused across all attributes,
                // Ex: User Agents like Chrome/etc have version number at the end which always would fail
                if (value.Contains(p, StringComparison.OrdinalIgnoreCase)) return true;
            }
        }

        return value == match;
    }

    protected void OnAccessDenied(ActionExecutingContext context, string message)
    {
        var result = new ContentResult();

        // TODO: Support plain/text and xml responses too
        result.Content = @"{ ""success"": false, ""status"": 403, ""type"": """ + this.GetType().Name + "\", \"error\": \"" + message + "\" }";

        result.ContentType = "application/json";

        result.StatusCode = 403;

        context.Result = result;
    }
}
