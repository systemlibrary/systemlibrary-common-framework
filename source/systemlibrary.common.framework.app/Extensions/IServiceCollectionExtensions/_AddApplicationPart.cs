using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

namespace SystemLibrary.Common.Framework.App.Extensions;

partial class IServiceCollectionExtensions
{
    static IMvcBuilder AddApplicationParts(IMvcBuilder builder, FrameworkServiceOptions options)
    {
        var executingAssembliy = Assembly.GetExecutingAssembly();
        var entryAssembly = Assembly.GetEntryAssembly();
        var callingAssembly = Assembly.GetCallingAssembly();

        builder = AddApplicationPart(builder, executingAssembliy, entryAssembly, callingAssembly);

        if (options.ApplicationParts != null)
        {
            foreach (var part in options.ApplicationParts)
                if (part != null &&
                    part != executingAssembliy &&
                    part != entryAssembly &&
                    part != callingAssembly)
                    builder = builder.AddApplicationPart(part);
        }

        return builder;
    }

    static IMvcBuilder AddApplicationPart(IMvcBuilder builder, Assembly executing, Assembly entry, Assembly calling)
    {
        if (builder != null)
        {
            if (executing != null)
                builder = builder.AddApplicationPart(executing);

            if (entry != null && executing?.FullName != entry.FullName)
                builder = builder.AddApplicationPart(entry);

            if (calling != null && executing?.FullName != calling.FullName && entry?.FullName != calling.FullName)
                builder = builder.AddApplicationPart(calling);
        }

        return builder;
    }
}