using System.Diagnostics;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

namespace SystemLibrary.Common.Framework.Tests;

public abstract class BaseTest
{
    static object ClientLock = new object();

    TestServer Server;
    HttpClient _Client;

    protected IWebHostBuilder WebHostBuilder;

    protected HttpClient Client
    {
        get
        {
            if (_Client == null)
            {
                lock (ClientLock)
                {
                    if (_Client != null) return Client;

                    Server = new TestServer(WebHostBuilder);

                    _Client = Server.CreateClient();
                }
            }
            return _Client;
        }
    }

    public string GetResponseText(string pathAndQuery)
    {
        var response = Client.GetAsync(pathAndQuery)
           .ConfigureAwait(false)
           .GetAwaiter()
           .GetResult();

        if (!response.IsSuccessStatusCode)
            Log.Dump("Not successful: " + pathAndQuery + " " + response.StatusCode);

        return response.Content.ReadAsStringAsync()
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();
    }

    protected HttpResponseMessage GetResponse(HttpRequestMessage request)
    {
        try
        {
            return Client.SendAsync(request)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
        }
        catch(Exception ex)
        {
            Log.Dump(ex);
            return default;
        }
    }

    protected string GetResponseText(HttpResponseMessage response)
    {
        return response.Content.ReadAsStringAsync()
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();
    }
}
