﻿using System.Text.Json.Serialization;

namespace SystemLibrary.Common.Framework;

internal class JsonConfig
{
    public bool AllowTrailingCommas { get; set; } = true;
    public int MaxDepth { get; set; } = 32;
    public bool PropertyNameCaseInsensitive { get; set; } = true;
    public JsonIgnoreCondition JsonIgnoreCondition { get; set; } = JsonIgnoreCondition.WhenWritingNull;
    public bool WriteIndented { get; set; } = false;
    public bool IgnoreReadOnlyFields { get; set; } = true;
    public bool IncludeFields { get; set; } = true;
    public bool JsonSecureAttributesEnabled { get; set; } = true;
}
