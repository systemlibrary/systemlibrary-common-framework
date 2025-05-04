using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework.App;

partial class Client
{
    partial class Request
    {
        static void SetRequestMessageHeaders(HttpRequestMessage message, RequestOptions options, ContentType contentType)
        {
            message.Headers.ExpectContinue = options.ExpectContinue;

            AddHeaders(message, options.Headers);

            message.Headers.TryAddWithoutValidation("Connection", "Keep-Alive");

            if (contentType != ContentType.None)
            {
                message.Headers.TryAddWithoutValidation("Accept", "*/*");
                message.Headers.TryAddWithoutValidation("Content-Type", contentType.ToValue());
            }
        }

        static void AddHeaders(HttpRequestMessage message, IDictionary<string, string> headers)
        {
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    if (header.Key.Is())
                        message.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }
        }
    }
}
