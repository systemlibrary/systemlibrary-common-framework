using System.Collections;
using System.Dynamic;
using System.Reflection;
using System.Text;
using System.Text.Json;

using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework.App;

partial class Client
{
    partial class Request
    {
        static void SetRequestMessageContent(HttpRequestMessage message, RequestOptions options, ContentType contentType)
        {
            var data = options.Data;

            if (data == null) return;

            message.Content = GetDataAsHttpContent(data, contentType) ??
                GetDataBytesAsHttpContent(data, contentType) ??
                GetFormUrlEncodedContent(data, contentType) ??
                GetDataStringAsHttpContent(options, contentType);
        }

        static HttpContent GetDataAsHttpContent(object data, ContentType contentType)
        {
            switch (data)
            {
                case FormUrlEncodedContent formUrlEncodedContent:
                    return formUrlEncodedContent;

                case ByteArrayContent byteArrayContent:
                    {
                        if (contentType != ContentType.None)
                            byteArrayContent.Headers.TryAddWithoutValidation("Content-Type", contentType.ToValue());

                        return byteArrayContent;
                    }

                case HttpContent httpContent:
                    return httpContent;

                case HttpRequestMessage msg:
                    return msg.Content;

                default:
                    return null;
            }
        }

        static HttpContent GetFormUrlEncodedContent(object data, ContentType contentType)
        {
            if (contentType != ContentType.xwwwformUrlEncoded) return null;

            if (data is ExpandoObject expando)
            {
                throw new Exception("Expando is currently not fully implemented in GetFormUrlEncodedDataAsHttpContent() for x-www-form-urlencoded, convert it yourself first to for instance IDictionary<string,string>");
            }

            if (data is List<KeyValuePair<string, string>> keyValuePairCollection)
                return new FormUrlEncodedContent(keyValuePairCollection);

            if (data is IDictionary dictionary)
            {
                var keyValuePairs = new List<KeyValuePair<string, string>>();

                foreach (DictionaryEntry keyValue in dictionary)
                    keyValuePairs.Add(new KeyValuePair<string, string>(keyValue.Key.ToString(), keyValue.Value?.ToString()));

                return new FormUrlEncodedContent(keyValuePairs);
            }

            if (data is string text)
            {
                var keyValues = new List<KeyValuePair<string, string>>();

                var inputs = text.Split('&');

                foreach (var input in inputs)
                {
                    if (input.IsNot()) continue;

                    var keyValue = input.Split('=');

                    if (keyValue.IsNot()) continue;

                    keyValues.Add(new KeyValuePair<string, string>(keyValue[0], keyValue.Length > 1 ? keyValue[1] : null));
                }

                return new FormUrlEncodedContent(keyValues);
            }

            var dataType = data.GetType();

            if (dataType.IsClass)
            {
                var properties = dataType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);

                var formProperties = new Dictionary<string, string>();

                if (properties?.Length > 0)
                {
                    foreach (var property in properties)
                    {
                        if (property.PropertyType.IsListOrArray() || property.PropertyType.IsGenericType)
                            throw new Exception("Class has a property " + property.Name + " which is either generic, list or an array. Not yet implemented in combination with wwwformUrlEncoded. Convert the class and its properties yourself to the data you need and set the content-type accordingly.");

                        var value = property.GetValue(data);

                        if (value != null)
                            formProperties.Add(property.Name, value.ToString());
                    }
                }

                if (formProperties.Count == 0)
                    throw new Exception("Class without properties to x-www-form-urlencoded string is not currently fully implemented in GetFormUrlEncodedContent(). Either your class is invalid or contains 0 properties, and properties must be 'public get'.");

                return new FormUrlEncodedContent(formProperties);
            }

            throw new Exception("x-www-form-urlencoded content-type requires data/payload to be either: List<KeyValuePair<string, string>> or IDictionary, or a string or a byte[] or a class with public get properties, or lastly, it can be null if the URL already contains the data key/values as query string and as url encoded form data");
        }
    }

    static HttpContent GetDataBytesAsHttpContent(object data, ContentType contentType)
    {
        if (contentType == ContentType.multipartFormData)
        {
            if (data is byte[] formDataBytes)
            {
                var content = new MultipartFormDataContent
                    {
                        { new StreamContent(new MemoryStream(formDataBytes)), "file" }
                    };

                return content;
            }
            throw new InvalidOperationException("Expected byte array data for multipart/form-data content type. Sending a file of any type the 'data/payload' must be a byte[] representing the file you are sending.");
        }

        if (data is byte[] bytes)
        {
            var byteContent = new ByteArrayContent(bytes, 0, bytes.Length);

            if (contentType != ContentType.None)
                byteContent.Headers.TryAddWithoutValidation("Content-Type", contentType.ToValue());

            return byteContent;
        }

        return null;
    }

    static HttpContent GetDataStringAsHttpContent(RequestOptions options, ContentType contentType)
    {
        var data = options.Data;

        if (data is string txt)
            return new StringContent(txt, Encoding.UTF8, contentType.ToValue());

        switch (contentType)
        {
            case ContentType.json:
                {
                    if (options.JsonSerializerOptions != null)
                    {
                        options.JsonSerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
                    }

                    return new StringContent(data.Json(options.JsonSerializerOptions), Encoding.UTF8, contentType.ToValue());
                }

            case ContentType.xml:
                return new StringContent(data.Xml(), Encoding.UTF8, contentType.ToValue());

            case ContentType.text:
                return new StringContent(data.ToString(), Encoding.UTF8, contentType.ToValue());

            case ContentType.svg:
                return new StringContent(data.Xml(), Encoding.UTF8, contentType.ToValue());

            default:
                throw new InvalidOperationException("Not yet implemented content-type " + contentType);
        }
    }
}
