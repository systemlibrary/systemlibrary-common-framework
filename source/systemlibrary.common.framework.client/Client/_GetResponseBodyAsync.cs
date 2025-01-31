using System.Net.Http;
using System.Threading.Tasks;

namespace SystemLibrary.Common.Framework.App;

public partial class Client
{
    static async Task<string> GetResponseBodyAsync(HttpResponseMessage response)
    {
        if (response?.Content == null) return default;

        //TODO: Support reading data as Stream/Files 
        using (response)
            return await response.Content
                .ReadAsStringAsync()
                .ConfigureAwait(false);
    }
}