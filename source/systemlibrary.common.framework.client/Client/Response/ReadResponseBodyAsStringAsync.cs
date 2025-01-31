using System.Net.Http;
using System.Threading.Tasks;

namespace SystemLibrary.Common.Framework.App;

partial class Client
{
    static async Task<string> ReadResponseBodyAsStringAsync(HttpResponseMessage response)
    {
        using (response)
            return await response.Content
                .ReadAsStringAsync()
                .ConfigureAwait(false);
    }
}
