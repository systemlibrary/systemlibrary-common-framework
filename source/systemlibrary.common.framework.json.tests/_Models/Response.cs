using System.Text.Json.Serialization;

namespace SystemLibrary.Common.Framework;

public class Response
{
    [JsonPropertyName("accept-language")]
    public string AcceptLanguage { get; set; }

    public string Host { get; set; }
}