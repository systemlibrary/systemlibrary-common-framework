using System.Text;

using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework.App;

partial class Client
{
    partial class Request
    {
        static HttpRequestMessage CreateHttpRequestMessage(RequestOptions options)
        {
            var message = new HttpRequestMessage(options.Method, options.Url);

            var contentType = GetContentType(options);

            Log.Dump(contentType);

            SetRequestMessageContent(message, options, contentType);
            Log.Dump("Set request message!");
            SetRequestMessageHeaders(message, options, contentType);
            Log.Dump("Set request headers!");
            return message;
        }
    }
}
