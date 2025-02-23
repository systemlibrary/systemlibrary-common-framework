using System.Net.Http;

using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework.App;

partial class Client
{
    partial class Request
    {
        static HttpRequestMessage CreateHttpRequestMessage(RequestOptions options)
        {
            var message = new HttpRequestMessage(options.Method, options.Url);

            message.Content = ClientHttpContent.Get(options);

            if (options.Headers != null)
            {
                foreach (var header in options.Headers)
                    if (header.Key.Is())
                        message.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            message.Headers.TryAddWithoutValidation("Accept", "*/*");
            message.Headers.TryAddWithoutValidation("Connection", "Keep-Alive");

            if (options.MediaType == MediaType.plain ||
                options.MediaType == MediaType.json ||
                options.MediaType == MediaType.html ||
                options.MediaType == MediaType.css ||
                options.MediaType == MediaType.pdf ||
                options.MediaType == MediaType.zip ||
                options.MediaType == MediaType.javascript ||
                options.MediaType == MediaType.xml)
                message.Headers.TryAddWithoutValidation("Content-Type", options.MediaType.ToValue());

            return message;
        }

    }
}
