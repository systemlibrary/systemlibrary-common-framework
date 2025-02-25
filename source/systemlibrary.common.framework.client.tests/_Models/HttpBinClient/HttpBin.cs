namespace SystemLibrary.Common.Framework.App.Tests;

class HttpBin : Client
{
    const string firewallClientUrl = "https://170.44.1.1/";
    const string clientUrl = "https://httpbin.org";

    public HttpBin(bool useRetryPolicy = false, int? timeout = null) : base(timeout, useRetryPolicy, null)
    {
    }

    public ClientResponse<string> Head()
    {
        return Head<string>("http://httpbin.org", timeoutMilliseconds: 3500);
    }

    public ClientResponse<string> Delete(object data, ContentType contentType)
    {
        return Delete<string>(clientUrl + "/delete", data, contentType);
    }

    public ClientResponse<string> Put(object data, ContentType contentType)
    {
        return Put<string>(clientUrl + "/put", data, contentType);
    }

    public ClientResponse<string> Get()
    {
        return Get<string>(clientUrl + "/get");
    }

    public ClientResponse<string> Post(object data, ContentType mediaType, Dictionary<string, string> headers = null)
    {
        return Post<string>(clientUrl + "/post", data, mediaType, headers: headers);
    }

    public ClientResponse<string> PostUrlEncoded(object data)
    {
        return Post<string>(clientUrl + "/post", data, ContentType.xwwwformUrlEncoded);
    }

    public async Task<ClientResponse<string>> PostAsync(string data)
    {
        return await PostAsync<string>(clientUrl + "/post", data, ContentType.text, null, 10000);
    }

    public ClientResponse<string> Get_Retry_Request_Against_Firewall(CancellationToken cancellationToken = default)
    {
        return Get<string>(firewallClientUrl, ContentType.json, null, 200, null, cancellationToken);
    }

    public ClientResponse<string> Post_Retry_Request_Against_Firewall()
    {
        return Post<string>(firewallClientUrl, "hello world", ContentType.json, null, 300);
    }

    public ClientResponse<string> GetWithCancellationToken(CancellationToken token)
    {
        return Get<string>(clientUrl + "/delay/2", ContentType.json, null, 4000, null, token);
    }

    public ClientResponse<string> GetWithTimeout(int timeoutMilliseconds, int sleep = 3)
    {
        return Get<string>(clientUrl + "/delay/" + sleep, ContentType.json, null, timeoutMilliseconds);
    }
}
