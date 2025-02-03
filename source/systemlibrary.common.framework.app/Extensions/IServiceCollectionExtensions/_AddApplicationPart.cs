using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

namespace SystemLibrary.Common.Framework.App.Extensions;

partial class IServiceCollectionExtensions
{
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