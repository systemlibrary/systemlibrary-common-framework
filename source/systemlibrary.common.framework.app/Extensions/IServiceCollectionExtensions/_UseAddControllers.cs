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
        var defaultJsonSerializerOptions = JsonSerializerOptionsInstance.Current(null);

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