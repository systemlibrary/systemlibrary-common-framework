using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SystemLibrary.Common.Framework.App;

// TODO:
// Add two more optional Handlers per client:
// CacheHandler, if added it checks ETag, Cache policy headers and if you send something with a cache header and it exists
// in IMemoryCache (I probably use the IMemoryCache instance here, not my own Cache.cs), we will see... a simplistic cache

// RequestLimitHandler, defined a maximum requests per minute of 120, so max 2 per seconds,
// optional to plugin and can modify the maximum requests per minute variable

partial class Client
{
    class TimeoutHandler : DelegatingHandler
    {
        TimeSpan RequestTimeoutSpan;

        public TimeoutHandler(int timeout, SocketsHttpHandler innerHandler) : base(innerHandler)
        {
            RequestTimeoutSpan = TimeSpan.FromMilliseconds(timeout);
        }

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            using (var source = GetTimeoutCancellationToken(cancellationToken))
            {
                return await base
                    .SendAsync(request, source.Token)
                    .ConfigureAwait(false);
            }
        }

        CancellationTokenSource GetTimeoutCancellationToken(CancellationToken cancellationToken)
        {
            var cancellationSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            if (IsTimeoutRegistered())
                cancellationSource.CancelAfter(RequestTimeoutSpan);

            return cancellationSource;
        }

        bool IsTimeoutRegistered()
        {
            return
                RequestTimeoutSpan != TimeSpan.MinValue &&
                RequestTimeoutSpan != TimeSpan.MaxValue &&
                RequestTimeoutSpan != System.Threading.Timeout.InfiniteTimeSpan;
        }
    }
}