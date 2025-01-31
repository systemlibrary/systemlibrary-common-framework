using System.Net.Http;
using System.Text;

using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework.App;

partial class Client
{
    partial class ClientHttpContent
    {
        static HttpContent GetBodyPlainText(object data, Encoding encoding = null, MediaType mediaType = MediaType.plain)
        {
            return new StringContent(data is string str ? str : data.ToString(), encoding ?? Encoding.UTF8, mediaType.ToValue());
        }
    }
}