using System.Text.Json;

namespace SystemLibrary.Common.Framework;

partial class PartialJsonSearcher
{
    static JsonElement GetRootElement(JsonDocumentOptions documentOptions, string[] propertyPaths, JsonDocument jsonDocument)
    {
        //TODO: Might get "RootElement" based on first "propertyName" in path, more efficient?
        var root = jsonDocument.RootElement;

        if (propertyPaths != null && propertyPaths.Length > 0)
        {
            foreach (var path in propertyPaths)
            {
                if (path.IsNot()) continue;

                var pathElement = GetJsonElement(root, path, 0, documentOptions.MaxDepth);
                if (pathElement.HasValue)
                    root = pathElement.Value;
                else
                    throw new Exception("Could not find property with name '" + path + "', cannot continue.");
            }
        }

        return root;
    }
}
