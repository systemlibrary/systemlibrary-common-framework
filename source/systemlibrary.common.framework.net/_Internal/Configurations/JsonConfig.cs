using System.Text.Json;
using System.Text.Json.Serialization;

namespace SystemLibrary.Common.Framework;

internal class JsonConfig
{
    public bool AllowTrailingCommas { get; set; } = true;
    public int MaxDepth { get; set; } = 32;
    public bool PropertyNameCaseInsensitive { get; set; } = true;
    public JsonCommentHandling JsonCommentHandling { get; set; } = JsonCommentHandling.Skip;
    public JsonIgnoreCondition JsonIgnoreCondition { get; set; } = JsonIgnoreCondition.WhenWritingNull;
    public bool WriteIndented { get; set; } = false;
    public bool IgnoreReadOnlyFields { get; set; } = true;
    public bool IncludeFields { get;set; } = true;
}
