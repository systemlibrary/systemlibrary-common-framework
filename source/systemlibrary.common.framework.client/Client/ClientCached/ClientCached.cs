using System.Collections.Concurrent;

namespace SystemLibrary.Common.Framework.App;

partial class Client
{
    //TODO: https://stackoverflow.com/questions/20661652/progress-bar-with-httpclient/69826649#69826649
    //A way to get "current percentage of downloaded file/stream/uploading..."
    //Would be awesome for any kind of react component, just showing a "progress spinner" instead of "random gif"
    //That just jumps whenever whatever, after validation 10%, before sending request to an integration 25%, after integration response 80%, then 100% right before sending response (or 99%)
    partial class ClientCached
    {
        const int ThresholdRegenerateClientSeconds = 60;
        static ConcurrentDictionary<string, CacheModel> Cache;
        static ConcurrentDictionary<string, CacheModel> DisposeQueue;

        static ClientCached()
        {
            Cache = new ConcurrentDictionary<string, CacheModel>();
            DisposeQueue = new ConcurrentDictionary<string, CacheModel>();
        }

        internal static HttpClient GetClient(RequestOptions options)
        {
            var uri = options.Uri;

            var key = $"{uri.Host.ToLowerInvariant()}{uri.Port}{options.GetTimeout()}{options.IgnoreSslErrors}Slcf{nameof(Client)}{nameof(GetClient)}{uri.Scheme.ToLowerInvariant()}";

            if (options.ForceNewClient)
            {
                var newlyCreatedClient = HasNewlyBeenCreated(key);
                if (newlyCreatedClient != null)
                {
                    return newlyCreatedClient;
                }

                RemoveFromCache(key, options.Timeout);
            }
            else if (Cache.TryGetValue(key, out CacheModel cachedModel))
            {
                if (HasExpired(cachedModel))
                    RemoveFromCache(key, options.Timeout);
                else
                {
                    return cachedModel.CachedClient;
                }
            }

            return New(key, options);
        }

        static HttpClient New(string key, RequestOptions options)
        {
            var socketsHandler = new SocketsHttpHandler
            {
                MaxConnectionsPerServer = 4096,

                // TODO: A policy to auto decompress or not, both in appSettings and per client
                //AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.Brotli,

                // Each http client's connection is reused for 540 seconds - 9 minutes
                // once reached, the connection is reestablished on next request no matter what
                PooledConnectionLifetime = TimeSpan.FromSeconds(540),
                // If a connection is idle for 40 seconds it is removed
                PooledConnectionIdleTimeout = TimeSpan.FromSeconds(40),
                // Establish TLS/a con within 13 seconds, else we retry at least once which adds up to 26 seconds
                ConnectTimeout = TimeSpan.FromSeconds(13),

                // TODO: A flag to "useRedirectPolicy" to true or false
                AllowAutoRedirect = true,
                EnableMultipleHttp2Connections = true,
                UseProxy = false
            };

            if (options.IgnoreSslErrors)
            {
                socketsHandler.SslOptions = new System.Net.Security.SslClientAuthenticationOptions()
                {
                    RemoteCertificateValidationCallback = (message, cert, chain, errors) =>
                    {
                        if (errors == System.Net.Security.SslPolicyErrors.RemoteCertificateChainErrors ||
                            errors == System.Net.Security.SslPolicyErrors.RemoteCertificateNameMismatch ||
                            errors == System.Net.Security.SslPolicyErrors.RemoteCertificateNotAvailable)
                        {
                            Log.Warning("Client: SslPolicy error occured, " + errors + ". Usually invalid or expired cert. IgnoreSslErrors is set to 'true' so continuing...");
                        }
                        return true;
                    }
                };
            }


            var timeoutHandler = new TimeoutHandler(options.GetTimeout(), socketsHandler);

            var client = new HttpClient(timeoutHandler, disposeHandler: true);

            // Adding 1.5s so it never triggers, our own TimeoutHandler triggers before the internal client timeout
            client.Timeout = TimeSpan.FromMilliseconds(int.Max(options.Timeout, options.RetryTimeout) + 1500);

            if (ClientCacheDurationConfig > 0)
            {
                var now = DateTime.Now;
                var cachedModel = new CacheModel()
                {
                    CachedClient = client,
                    ThresholdRegenerateClient = now.AddSeconds(ThresholdRegenerateClientSeconds),
                    Expires = now.AddSeconds(ClientCacheDurationConfig)
                };

                if (!Cache.TryAdd(key, cachedModel))
                {
                    if (Cache.TryGetValue(key, out CacheModel existingInCacheModel))
                    {
                        if (!HasExpired(existingInCacheModel))
                        {
                            cachedModel.CachedClient = null;
                            try
                            {
                                client.Dispose();
                            }
                            catch
                            {
                                // Swallow
                            }
                            client = null;
                            cachedModel = null;

                            return existingInCacheModel.CachedClient;
                        }
                        else
                        {
                            // Edge case, if client was not added, but also was expired
                            // at least we return a new client ready to be used, but it will leak this one
                        }
                    }
                }
            }

            Debug.Log("New client " + key);

            return client;
        }

        static bool HasExpired(CacheModel httpClientCached)
        {
            return httpClientCached?.CachedClient == null || httpClientCached.Expires < DateTime.Now;
        }

        static void RemoveFromCache(string key, int timeoutMilliseconds)
        {
            if (Cache.TryRemove(key, out CacheModel cachedModel))
            {
                Dispose();

                if (cachedModel != null)
                {
                    cachedModel.Expires = DateTime.Now.AddMilliseconds(timeoutMilliseconds + 600000);
                    DisposeQueue.TryAdd(key + DateTime.Now.ToString("HH:mm:ss.fffff") + "#" + Randomness.Int() + Randomness.String(6), cachedModel);
                    Debug.Log("Moved client to dispose queue: " + key);
                }
            }
        }

        static HttpClient HasNewlyBeenCreated(string key)
        {
            if (ClientCacheDurationConfig <= 0) return null;

            if (Cache.TryGetValue(key, out CacheModel cached))
            {
                if (!HasExpired(cached) && cached.ThresholdRegenerateClient > DateTime.Now)
                {
                    return cached.CachedClient;
                }
            }

            return null;
        }
    }
}