using Microsoft.Extensions.DependencyInjection;

namespace SystemLibrary.Common.Framework.App.Extensions;

partial class IServiceCollectionExtensions
{
    static IMvcBuilder UseDefaultJsonConverters(this IMvcBuilder builder)
    {
        if (builder == null) return builder;

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
            options.JsonSerializerOptions.IgnoreReadOnlyFields = defaultJsonSerializerOptions.IgnoreReadOnlyFields;
            options.JsonSerializerOptions.IncludeFields = defaultJsonSerializerOptions.IncludeFields;

            foreach (var converter in defaultJsonSerializerOptions.Converters)
            {
                options.JsonSerializerOptions.Converters.Add(converter);
            }
        });
    }
}