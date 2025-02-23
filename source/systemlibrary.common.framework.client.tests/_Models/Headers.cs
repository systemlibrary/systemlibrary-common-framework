using System.Text.Json.Serialization;

namespace SystemLibrary.Common.Framework.App.Tests;

public class Headers
{
    [JsonPropertyName("Content-Length")]
    public int ContentLength { get; set; }

    [JsonPropertyName("Content-Type")]
    public string ContentType { get; set; }
}
