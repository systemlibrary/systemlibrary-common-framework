using System.Net.Http;
using System.Text;
using System.Text.Json;

using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework.App;

partial class Client
{
    partial class ClientHttpContent
    {
        static HttpContent GetBodyJson(object data, Encoding encoding = null, JsonSerializerOptions jsonSerializerOptions = null, MediaType mediaType = MediaType.json)
        {
            return data is string text
                ? new StringContent(text, encoding ?? Encoding.UTF8, mediaType.ToValue())
                : new StringContent(data.Json(jsonSerializerOptions), encoding ?? Encoding.UTF8, mediaType.ToValue());
        }
    }
}