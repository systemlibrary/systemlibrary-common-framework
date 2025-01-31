using System.Text.Json;

namespace SystemLibrary.Common.Framework;

partial class PartialJsonSearcher
{
    static JsonElement? GetJsonElement(JsonElement curr, string findName, int depth, int maxDepth)
    {
        if (depth > maxDepth) return null;

        findName = findName.ToLower();

        if (curr.ValueKind == JsonValueKind.Object)
        {
            foreach (var property in curr.EnumerateObject())
            {
                if (property.Name.ToLower() == findName)
                {
                    return property.Value;
                }

                var found = GetJsonElement(property.Value, findName, depth++, maxDepth);

                if (found != null) return found;
            }
        }
        else if (curr.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in curr.EnumerateArray())
            {
                var found = GetJsonElement(item, findName, depth++, maxDepth);
                if (found != null) return found;
            }
        }
        return null;
    }
}
