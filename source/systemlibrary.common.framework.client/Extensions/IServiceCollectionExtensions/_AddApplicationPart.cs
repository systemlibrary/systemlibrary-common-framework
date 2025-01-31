using System;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

namespace SystemLibrary.Common.Framework.App.Extensions;

partial class IServiceCollectionExtensions
{
    static IMvcBuilder AddApplicationPart(IMvcBuilder builder, FrameworkServicesOptions options, Assembly executing, Assembly entry)
    {
        if (builder != null)
        {
            if (executing != null)
                builder = builder.AddApplicationPart(executing);

            if (entry != null && executing?.FullName != entry.FullName)
                builder = builder.AddApplicationPart(entry);
        }
        else if (options.AdditionalSupportedMediaTypes != null)
            throw new Exception("UseMvc, UseRazorPages and UseControllers are all false, yet you've set AdditionalSupportedMediaTypes or added ApplicationParts (assemblies). Set one of the flags to true to continue.");

        return builder;
    }
}