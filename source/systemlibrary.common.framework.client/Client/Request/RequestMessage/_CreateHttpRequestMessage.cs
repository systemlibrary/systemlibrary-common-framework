namespace SystemLibrary.Common.Framework.App;

partial class Client
{
    partial class Request
    {
        static HttpRequestMessage CreateHttpRequestMessage(RequestOptions options)
        {
            var message = new HttpRequestMessage(options.Method, options.Url);

            var contentType = GetContentType(options);

            SetRequestMessageContent(message, options, contentType);

            SetRequestMessageHeaders(message, options, contentType);

            return message;
        }
    }
}
