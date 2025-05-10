using System.Reflection;
using System.Security.Claims;
using System.Text;

using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.DependencyInjection;

using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework.App.Extensions;

static partial class IServiceCollectionExtensions
{
    static string[] VaryCacheByQueryNames = new[] { "id", "country", "sub", "subscription", "code", "lang", "type", "q", "format", "version", "v", "category", "quality", "sort", "sortBy", "sorting", "order", "orderBy", "page", "pageNumber", "pageIndex", "filter", "filterBy" };

    static IServiceCollection UseOutputCache(this IServiceCollection services, FrameworkOptions options)
    {
        if (!options.UseOutputCache) return services;

        services.AddScoped<OutputCacheTagFilter>();

        return services.AddOutputCache(options =>
        {
            options.SizeLimit = 4000L * 1024 * 1024;       //4GB
            options.MaximumBodySize = 8 * 1024 * 1024;     //8MB
            options.UseCaseSensitivePaths = false;
            options.DefaultExpirationTimeSpan = TimeSpan.FromSeconds(1200); // CacheDuration.M

            options.AddBasePolicy(policy =>
            {
                policy.VaryByValue(context =>
                {
                    var isAuthenticated = (context?.User?.Identity?.IsAuthenticated ?? false);

                    return new("slcf__isauth", isAuthenticated.ToString());
                });

                policy.SetVaryByQuery(VaryCacheByQueryNames);

                policy.Expire(TimeSpan.FromSeconds(1200)); // CacheDuration.M
            });

            options.AddPolicy(CachePolicy.CacheAuthenticated.ToValue(), policy =>
            {
                policy.VaryByValue(context =>
                {
                    return new("slcf__isauth", (context?.User?.Identity?.IsAuthenticated ?? false).ToString());
                });

                policy.SetVaryByQuery(VaryCacheByQueryNames);

                policy.Expire(TimeSpan.FromSeconds(1200)); // CacheDuration.M
            });

            options.AddPolicy(CachePolicy.CacheRoles.ToValue(), policy =>
            {
                policy.VaryByValue(context =>
                {
                    var user = context?.User;

                    var isAuth = user?.Identity?.IsAuthenticated ?? false;
                    if (!isAuth)
                        return new("slcf__r", "false");

                    var claimsIdentity = user.Identity as ClaimsIdentity;

                    var cacheKey = new StringBuilder("", 128);

                    if (user?.Claims != null)
                    {
                        var roles = user.Claims
                            .Where(c => c.Type == claimsIdentity.RoleClaimType ||
                                    c.Type.Equals("role", StringComparison.OrdinalIgnoreCase))
                            .Select(x => x.Value);

                        var role = user.FindFirst(ClaimTypes.Role)?.Value;

                        if (roles.Is())
                            return new("slcf__r", string.Join("", roles) + role);

                        if (role.Is())
                            return new("slcf__r", role);
                    }
                    return new("slcf__r", "true");
                });

                policy.SetVaryByQuery(VaryCacheByQueryNames);

                policy.Expire(TimeSpan.FromSeconds(1200)); // CacheDuration.M
            });

            options.AddPolicy(CachePolicy.CacheUser.ToValue(), policy =>
            {
                policy.VaryByValue(context =>
                {
                    var user = context?.User;

                    var isAuth = user?.Identity?.IsAuthenticated ?? false;
                    if (!isAuth)
                        return new("slcf__user", "false");

                    var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (userId.IsNot())
                        userId = user.FindFirst("id")?.Value ?? user.Identity.Name;

                    var email = user.FindFirst(ClaimTypes.Email)?.Value;
                    if (email.IsNot())
                        email = user.FindFirst("email")?.Value ?? user.FindFirst("emailaddress")?.Value;

                    var phone = user.FindFirst(ClaimTypes.MobilePhone)?.Value;
                    if (phone.IsNot())
                        phone = user.FindFirst("phone")?.Value ?? user.FindFirst("phonenumber")?.Value ?? user.FindFirst("mobile")?.Value;

                    var sub = user.FindFirst("sub")?.Value;

                    if (sub.IsNot())
                        sub = user.FindFirst("user_id")?.Value;

                    var cacheKey = userId + email + phone + sub;

                    if (cacheKey.IsNot() || cacheKey.Length < 5)
                        return new("slcf__user", "false");

                    return new("slcf__user", cacheKey);
                });

                policy.SetVaryByQuery(VaryCacheByQueryNames);

                policy.Expire(TimeSpan.FromSeconds(1200)); // CacheDuration.M
            });
        });
    }
}

internal class OutputCacheTagFilter : IAsyncActionFilter
{
    IOutputCacheStore _cacheStore;

    public OutputCacheTagFilter(IOutputCacheStore cacheStore)
    {
        _cacheStore = cacheStore;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var user = context?.HttpContext?.User;

        var isAuthenticated = (user?.Identity?.IsAuthenticated ?? false);

        if (!isAuthenticated)
        {
            await next();
            return;
        }

        var outputCacheAttribute = context.ActionDescriptor.EndpointMetadata.OfType<OutputCacheAttribute>().FirstOrDefault();
        var tags = outputCacheAttribute?.Tags;

        if (tags.IsNot())
        {
            await next();
            return;
        }

        var skipWhenAuthenticated = tags.Any(t => t?.Contains("skipWhenAuthenticated=true", StringComparison.OrdinalIgnoreCase) == true);

        if (skipWhenAuthenticated)
        {
            DisableOutputCacheDuration(outputCacheAttribute);
        }
        else
        {
            var skipWhenAdmin = tags.Any(t => t?.Contains("skipWhenAdmin=true", StringComparison.OrdinalIgnoreCase) == true);
            if (skipWhenAdmin)
            {
                var isAdmin = user.IsInAnyRole("administrator", "admin", "administrators", "admins", "Administrators", "Administrator", "Admins", "Admin");

                if (isAdmin)
                {
                    DisableOutputCacheDuration(outputCacheAttribute);
                }
            }
        }

        await next();
    }

    static void DisableOutputCacheDuration(OutputCacheAttribute outputCacheAttribute)
    {
        var durationField = typeof(OutputCacheAttribute).GetField("_duration", BindingFlags.NonPublic | BindingFlags.Instance);
        if (durationField != null)
        {
            durationField.SetValue(outputCacheAttribute, 0);
        }
        else
        {
            Log.Error("Duration was not reset, the _duration field does no longer exist in the output Cache");
        }
    }
}