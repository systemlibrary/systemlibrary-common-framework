using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;

namespace SystemLibrary.Common.Framework.App.Extensions;

static partial class IServiceCollectionExtensions
{
    static IServiceCollection UseViews(this IServiceCollection services, FrameworkServiceOptions options = null)
    {
        return options.ViewLocationExpander == null && options.ViewLocations == null && options.AreaViewLocations == null
            ? services
            : services.Configure<RazorViewEngineOptions>(razorViews =>
        {
            if (options.ViewLocationExpander != null)
                razorViews.ViewLocationExpanders.Add(options.ViewLocationExpander);

            if (options.AreaViewLocations != null)
            {
                foreach (var view in options.AreaViewLocations)
                {
                    if (view.Is())
                        razorViews.AreaViewLocationFormats.Add(view);
                }
            }

            if (options.ViewLocations != null)
            {
                foreach (var view in options.ViewLocations)
                {
                    if (view.Is())
                        razorViews.ViewLocationFormats.Add(view);
                }
            }
        });
    }
}