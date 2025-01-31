using System;
using System.Linq;
using System.Reflection;
using System.Text.Json;

using Microsoft.Extensions.DependencyInjection;

namespace SystemLibrary.Common.Framework.App.Extensions;

partial class IServiceCollectionExtensions
{
    static IMvcBuilder UseAddControllers(this IServiceCollection services, FrameworkServicesOptions options)
    {
        return services.AddControllers(ConfigureSupportedMediaTypes(options));
    }

    static IMvcBuilder UseDefaultJsonConverters(this IMvcBuilder builder)
    {
        var type = Type.GetType("SystemLibrary.Common.Framework._JsonSerializerOptions, SystemLibrary.Common.Framework.Net");

        if (type == null)
            throw new Exception("SystemLibrary.Common.Framework._JsonSerializerOptions is not loaded or type is renamed in version you are using, either upgrade or downgrade SystemLibrary.Common.Framework");

        var defaultJsonSerializerOptionMethod = type.GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
            .Where(x => x.Name == "Default")
            .FirstOrDefault();

        if (defaultJsonSerializerOptionMethod == null)
            throw new Exception("SystemLibrary.Common.Framework._JsonSerializerOptions no longer contains method 'Default', not a compatible version, either upgrade or downgrade SystemLibrary.Common.Framework");

        var defaultJsonSerializerOptions = (JsonSerializerOptions)defaultJsonSerializerOptionMethod.Invoke(null, new object[] { null, null });

        return builder.AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Encoder = defaultJsonSerializerOptions.Encoder;
            options.JsonSerializerOptions.AllowTrailingCommas = defaultJsonSerializerOptions.AllowTrailingCommas;
            options.JsonSerializerOptions.DefaultIgnoreCondition = defaultJsonSerializerOptions.DefaultIgnoreCondition;
            options.JsonSerializerOptions.WriteIndented = defaultJsonSerializerOptions.WriteIndented;
            options.JsonSerializerOptions.PropertyNameCaseInsensitive = defaultJsonSerializerOptions.PropertyNameCaseInsensitive;
            options.JsonSerializerOptions.ReadCommentHandling = defaultJsonSerializerOptions.ReadCommentHandling;
            options.JsonSerializerOptions.ReferenceHandler = defaultJsonSerializerOptions.ReferenceHandler;
            options.JsonSerializerOptions.NumberHandling = defaultJsonSerializerOptions.NumberHandling;
            options.JsonSerializerOptions.UnknownTypeHandling = defaultJsonSerializerOptions.UnknownTypeHandling;

            foreach (var converter in defaultJsonSerializerOptions.Converters)
            {
                options.JsonSerializerOptions.Converters.Add(converter);
            }
        });
    }
}