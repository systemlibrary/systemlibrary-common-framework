using System.Net.Http;
using System.Threading.Tasks;

namespace SystemLibrary.Common.Framework.App;

partial class Client
{
    partial class Request
    {
        internal static async Task<HttpResponseMessage> SendAsync(RequestOptions options)
        {
            var message = CreateHttpRequestMessage(options);

            var client = ClientCached.GetClient(options);

            using (message)
                return await client.SendAsync(message, options.CancellationToken).ConfigureAwait(false);
        }
    }
}
