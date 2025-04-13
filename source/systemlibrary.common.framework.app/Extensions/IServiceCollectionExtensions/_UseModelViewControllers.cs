using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework.App.Extensions;

partial class IServiceCollectionExtensions
{
    static IMvcBuilder UseModelViewControllers(this IServiceCollection services, FrameworkOptions options)
    {
        if (options.UseMvc)
        {
            return services.AddMvc(opt =>
            {
                opt.AllowEmptyInputInBodyModelBinding = false;

                opt.CacheProfiles.Add("Default", new CacheProfile { Duration = 180, Location = ResponseCacheLocation.Any, VaryByHeader = "Accept-Language" });

                opt.OutputFormatters.Add(new OutputContentTypesSupported());

                if (options.UseControllers)
                    opt.Conventions.Add(new UseApiControllersRouting());
            });
        }

        if (options.UseControllers)
        {
            return services.AddControllers(options =>
            {
                options.Conventions.Add(new UseApiControllersRouting());
            });
        }


        return default;
    }
}