using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

using SystemLibrary.Common.Framework;
using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework.App;

partial class Client
{
    static Type HttpResponseMessageType = typeof(HttpResponseMessage);

    static async Task<T> ReadResponseAsync<T>(Type type, string url, HttpResponseMessage response, CancellationToken cancellationToken, JsonSerializerOptions jsonSerializerOptions)
    {
        if (response?.Content == null) return default;

        if (type == HttpResponseMessageType)
        {
            return (T)(object)response;
        }

        if (type.IsValueType || type == SystemType.StringType)
        {
            var body = await ReadResponseBodyAsStringAsync(response).ConfigureAwait(false);

            response.Dispose();

            if (body == null)
                return default;

            if (type == SystemType.StringType)
                return (T)(object)body;

            else if (type == SystemType.IntType)
                return (T)(object)Convert.ToInt32(body);

            else if (type == SystemType.BoolType)
                return bool.TryParse(body, out bool value) ? (T)(object)value : default;

            else if (type == SystemType.DateTimeType)
                return (T)(object)body.ToDateTime();

            else if (type == SystemType.DateTimeOffsetType)
                return (T)(object)body.ToDateTimeOffset();

            else
                throw new Exception("Type: " + type.Name + " is not yet implemented for method ReadResponseAsync()");
        }

        var contentType = response.Content.Headers?.ContentType?.MediaType;

        var isJson = contentType == null || contentType.Contains("json", StringComparison.OrdinalIgnoreCase);

        if (isJson)
        {
            using (response)
            using (var contentStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                return await contentStream.JsonAsync<T>(jsonSerializerOptions, cancellationToken);
            //return await JsonSerializer.DeserializeAsync<T>(contentStream, jsonSerializerOptions, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            var isXml = contentType.Contains("xml", StringComparison.OrdinalIgnoreCase);
            if (isXml)
            {
                var xml = await ReadResponseBodyAsStringAsync(response).ConfigureAwait(false);

                if (xml.IsNot()) return default;

                using (var reader = new StringReader(xml))
                {
                    var xmlSerializer = new XmlSerializer(type);
                    try
                    {
                        return (T)xmlSerializer.Deserialize(reader);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException("Failed to deserialize XML content starting with: " + xml.MaxLength(8), ex);
                    }
                }
            }

            throw new Exception("Cannot deserialize " + type.Name + " from the response body. Set the generic return type to be <HttpResponseMessage>() and parse the content yourself, targetting url: " + url);
        }
    }
}
