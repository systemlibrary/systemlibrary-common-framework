using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace SystemLibrary.Common.Framework;

static internal class JsonSerializerOptionsInstance
{
    static IntJsonConverter IntJsonConverter = new IntJsonConverter();
    static DateTimeJsonConverter DateTimeJsonConverter = new DateTimeJsonConverter("yyyy-MM-dd HH:mm:ss");
    static DateTimeOffsetJsonConverter DateTimeOffsetJsonConverter = new DateTimeOffsetJsonConverter("yyyy-MM-dd");
    static TypeConverter TypeConverter = new TypeConverter();
    static LongJsonConverter LongJsonConverter = new LongJsonConverter();
    static DelegateJsonConverter DelegateJsonConverter = new DelegateJsonConverter();

    internal static JsonSerializerOptions Current(JsonSerializerOptions options, params JsonConverter[] converters)
    {
        if (options == null && converters == null) return CreateNewDefaultOptions;

        if (options == null)
        {
            options = CreateNewDefaultOptions;

            foreach (var converter in converters)
                options.Converters.Insert(0, converter);
        }
        else
        {
            AddConverters(options);

            if (options.ReferenceHandler == null ||
                options.MaxDepth <= 0 ||
                options.PropertyNamingPolicy == null ||
                options.Encoder == null)
            {
                var tmp = CreateNewDefaultOptions;

                if(options.ReferenceHandler == null)
                options.ReferenceHandler = tmp.ReferenceHandler;

                if (options.MaxDepth <= 0)
                    options.MaxDepth = tmp.MaxDepth;

                if (options.PropertyNamingPolicy == null)
                    options.PropertyNamingPolicy = tmp.PropertyNamingPolicy;

                if(options.Encoder == null)
                    options.Encoder = tmp.Encoder;
            }
        }

        return options;
    }

    static void AddConverters(JsonSerializerOptions options)
    {
        if (options.Converters?.Count > 0) return;

        // NOTE: Optimize by creating these converters just once during app run time, they can be singleton IIRC
        options.Converters.Add(new StringJsonConverter());
        options.Converters.Add(IntJsonConverter);
        options.Converters.Add(new EnumStringConverterFactory());
        options.Converters.Add(new JsonStringEnumConverter());
        options.Converters.Add(DateTimeJsonConverter);
        options.Converters.Add(DateTimeOffsetJsonConverter);
        options.Converters.Add(LongJsonConverter);
        options.Converters.Add(TypeConverter);
        options.Converters.Add(DelegateJsonConverter);
    }

    static JsonConfig _JsonConfigInstance;
    static JsonConfig JsonConfigInstance
    {
        get
        {
            _JsonConfigInstance ??= FrameworkConfig.Current.Json;
            return _JsonConfigInstance;
        }
    }

    static JavaScriptEncoder _JavaScriptEncoder;
    static JavaScriptEncoder JavaScriptEncoder
    {
        get
        {
            _JavaScriptEncoder ??= JavaScriptEncoder.Create(
                            UnicodeRanges.BasicLatin,
                            UnicodeRanges.LatinExtendedA,
                            UnicodeRanges.LatinExtendedB,
                            UnicodeRanges.LatinExtendedAdditional,
                            UnicodeRanges.LatinExtendedC,
                            UnicodeRanges.Latin1Supplement,
                            UnicodeRanges.CurrencySymbols,
                            UnicodeRanges.Cyrillic,
                            UnicodeRanges.GreekandCoptic);
            return _JavaScriptEncoder;
        }
    }

    static JsonSerializerOptions CreateNewDefaultOptions
    {
        get
        {
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder,
                DefaultIgnoreCondition = JsonConfigInstance.JsonIgnoreCondition,
                MaxDepth = JsonConfigInstance.MaxDepth,
                AllowTrailingCommas = JsonConfigInstance.AllowTrailingCommas,
                PropertyNameCaseInsensitive = JsonConfigInstance.PropertyNameCaseInsensitive,
                WriteIndented = JsonConfigInstance.WriteIndented,
                PropertyNamingPolicy = null,
                IncludeFields = true,
                ReadCommentHandling = JsonConfigInstance.ReadCommentHandling,
                NumberHandling = JsonNumberHandling.AllowReadingFromString,
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
            };

            AddConverters(options);

            return options;
        }
    }
}
