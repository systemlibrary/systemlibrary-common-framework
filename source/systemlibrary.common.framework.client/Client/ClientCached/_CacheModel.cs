using System;
using System.Net.Http;

namespace SystemLibrary.Common.Framework.App;

partial class Client
{
    class CacheModel
    {
        public HttpClient CachedClient;
        public DateTime Expires;
        public DateTime ThresholdRegenerateClient;

        public void Dispose()
        {
            CachedClient?.Dispose();
            CachedClient = null;
        }
    }
}