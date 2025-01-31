using System.Text.Json;

namespace SystemLibrary.Common.Framework;

internal static partial class PartialJsonSearcher
{
    internal static T Search<T>(string json, string propertySearchPath = null, JsonSerializerOptions options = null, bool returnPropertyValue = false)
    {
        if (json.IsNot()) return default;

        options = JsonSerializerOptionsInstance.Current(options);

        var documentOptions = GetJsonDocumentOptions(options);

        var jsonDocument = JsonDocument.Parse(json, documentOptions);

        if (jsonDocument == null) return default;

        (string property, string[] propertyPaths) = SplitPropertyAndPropertyPath<T>(propertySearchPath);

        JsonElement root = GetRootElement(documentOptions, propertyPaths, jsonDocument);

        var jsonElement = GetJsonElement(root, property, 0, documentOptions.MaxDepth);

        if (!jsonElement.HasValue) return default;

        var value = jsonElement.ToString();

        if (value.IsNot()) return default;

        var type = typeof(T);
        if (type == SystemType.StringType)
            return (T)(object)value;

        if (type == SystemType.BoolType)
            return (T)(object)bool.Parse(value);

        if (type == SystemType.IntType)
            return (T)(object)int.Parse(value);

        if (type == SystemType.Int64Type)
            return (T)(object)long.Parse(value);

        if (type == SystemType.Int16Type)
            return (T)(object)short.Parse(value);

        if (type == SystemType.DateTimeType)
            return (T)(object)value.ToDateTime();

        if (type == SystemType.DateTimeOffsetType)
            return (T)(object)value.ToDateTimeOffset();

        if (!type.IsClass)
            throw new Exception("Not yet implemented deserialization to " + type.Name);

        if (!value.IsJson())
        {
            Debug.Log("PartialJsonSearcher.Search value is not JSON format: " + value);

            return default;
        }

        return JsonSerializer.Deserialize<T>(value, options);
    }
}
