using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace SystemLibrary.Common.Framework.App.Extensions;

static partial class IServiceCollectionExtensions
{
    static IServiceCollection UseCookiePolicy(this IServiceCollection services)
    {
     

        return services.Configure<CookiePolicyOptions>(options =>
        {
            options.HttpOnly = HttpOnlyPolicy.None;

            options.Secure = CookieSecurePolicy.SameAsRequest;
            options.CheckConsentNeeded = context => false;

            options.MinimumSameSitePolicy = SameSiteMode.Lax;
        });
    }
}