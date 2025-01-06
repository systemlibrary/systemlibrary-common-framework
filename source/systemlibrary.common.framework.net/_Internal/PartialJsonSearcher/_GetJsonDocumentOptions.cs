using System.Text.Json;

namespace SystemLibrary.Common.Framework;

partial class PartialJsonSearcher
{
    static JsonDocumentOptions GetJsonDocumentOptions(JsonSerializerOptions options)
    {
        return new JsonDocumentOptions
        {
            CommentHandling = options.ReadCommentHandling,
            AllowTrailingCommas = options.AllowTrailingCommas,
            MaxDepth = options.MaxDepth
        };
    }
}
