using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SystemLibrary.Common.Framework.App.Tests;

class HttpBin : Client
{
    const string firewallClientUrl = "https://170.44.1.1/";
    const string clientUrl = "https://httpbin.org";

    public HttpBin(bool useRetryPolicy = false, int? timeout = null) : base(timeout, useRetryPolicy, null)
    {
    }

    public ClientResponse<string> Head(MediaType mediaType)
    {
        return Head<string>("http://httpbin.org", mediaType, timeoutMilliseconds: 3500);
    }

    public ClientResponse<string> Delete(object data, MediaType mediaType)
    {
        return Delete<string>(clientUrl + "/delete", data, mediaType);
    }

    public ClientResponse<string> Put(object data, MediaType mediaType)
    {
        return Put<string>(clientUrl + "/put", data, mediaType);
    }

    public ClientResponse<string> Get()
    {
        return Get<string>(clientUrl + "/get");
    }

    public ClientResponse<string> Post(object data, MediaType mediaType, Dictionary<string, string> headers = null)
    {
        return Post<string>(clientUrl + "/post", data, mediaType, headers: headers);
    }

    public ClientResponse<string> PostUrlEncoded(object data)
    {
        return Post<string>(clientUrl + "/post", data, MediaType.xwwwformUrlEncoded);
    }

    public async Task<ClientResponse<string>> PostAsync(string data)
    {
        return await PostAsync<string>(clientUrl + "/post", data, MediaType.plain, null, 10000);
    }

    public ClientResponse<string> Get_Retry_Request_Against_Firewall(CancellationToken cancellationToken = default)
    {
        return Get<string>(firewallClientUrl, MediaType.json, null, 200, null, cancellationToken);
    }

    public ClientResponse<string> Post_Retry_Request_Against_Firewall()
    {
        return Post<string>(firewallClientUrl, "hello world", MediaType.json, null, 300);
    }

    public ClientResponse<string> GetWithCancellationToken(CancellationToken token)
    {
        return Get<string>(clientUrl + "/delay/2", MediaType.json, null, 4000, null, token);
    }

    public ClientResponse<string> GetWithTimeout(int timeoutMilliseconds, int sleep = 3)
    {
        return Get<string>(clientUrl + "/delay/" + sleep, MediaType.json, null, timeoutMilliseconds);
    }
}
