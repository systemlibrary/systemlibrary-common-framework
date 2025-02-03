using Microsoft.Extensions.DependencyInjection;

namespace SystemLibrary.Common.Framework.App.Extensions;

partial class IServiceCollectionExtensions
{
    static IMvcBuilder AddRazorRuntimeCompilationOnChange(IMvcBuilder builder)
    {
        if (builder != null)
            return builder.AddRazorRuntimeCompilation();
        else
            throw new System.Exception("RazorRuntimeCompilation was not registered, as you've set Controllers, Mvc and RazorPages to false. You must manually register it yourself");
    }
}