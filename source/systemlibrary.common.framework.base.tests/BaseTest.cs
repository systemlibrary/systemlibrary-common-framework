using System.Diagnostics;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

namespace SystemLibrary.Common.Framework.Tests;

public abstract partial class BaseTest
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
}
