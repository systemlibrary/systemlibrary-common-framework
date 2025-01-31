using System.Net.Http;
using System.Text;
using System.Text.Json;

using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework.App;

partial class Client
{
    partial class ClientHttpContent
    {
        internal static HttpContent Get(RequestOptions options)
        {
            HttpContent content = null;

            if (options.Data == null) return content;

            var data = options.Data;
            var mediaType = options.MediaType;

            if (data is FormUrlEncodedContent formUrlEncodedContent)
            {
                return formUrlEncodedContent;
            }
            else if (data is ByteArrayContent byteArrayContent)
            {
                if (mediaType != MediaType.None)
                    byteArrayContent.Headers.TryAddWithoutValidation("Content-Type", mediaType.ToValue());

                return byteArrayContent;
            }
            else if (data is HttpContent httpContent)
            {
                return httpContent;
            }
            else if (mediaType != MediaType.multipartFormData && data is byte[] bytes)
            {
                var byteContent = new ByteArrayContent(bytes, 0, bytes.Length);

                if (mediaType != MediaType.None)
                    byteContent.Headers.TryAddWithoutValidation("Content-Type", mediaType.ToValue());

                return byteContent;
            }
            else if (data is HttpRequestMessage msg)
            {
                return msg.Content;
            }

            switch (mediaType)
            {
                case MediaType.plain:
                    content = GetBodyPlainText(data, null, mediaType);
                    break;

                case MediaType.json:
                    if (options.JsonSerializerOptions != null)
                        options.JsonSerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;

                    content = GetBodyJson(data, null, options.JsonSerializerOptions, mediaType);
                    break;
                // What about XML?

                case MediaType.xwwwformUrlEncoded:
                    content = GetBodyXwwwFormUrlEncoded(data);
                    break;

                case MediaType.multipartFormData:
                    content = GetBodyMultipartFormData(data);
                    break;

                case MediaType.octetStream:
                    throw new System.Exception("Not yet implemented: " + mediaType + ", pass octet stream as byte array or HttpContent");
                case MediaType.html:
                case MediaType.css:
                case MediaType.javascript:
                case MediaType.pdf:
                case MediaType.zip:
                    throw new System.Exception("Not yet implemented: " + mediaType);

                //TODO: Consider binary json formatter, mediaType application/bson
                //MediaTypeFormatter bsonFormatter = new BsonMediaTypeFormatter();

                default:
                    content = new StringContent(data is string ? data as string : data.ToString(), Encoding.UTF8, mediaType.ToValue());
                    break;
            }

            return content;
        }
    }
}
