using System;
using System.Collections.Concurrent;
using System.Net.Http;

using Polly;

namespace SystemLibrary.Common.Framework.App;

internal static class RequestBreaker
{
    internal const int BreakOnExceptionsInRow = 25;
    internal const int BrokenDuration = 7;
    static ConcurrentDictionary<string, IAsyncPolicy> Policies = new ConcurrentDictionary<string, IAsyncPolicy>();

    internal static IAsyncPolicy GetPolicy(string policyKey)
    {
        return Policies.GetOrAdd(policyKey, CreatePolicy());
    }

    static IAsyncPolicy CreatePolicy()
    {
        return Policy.Handle<HttpRequestException>()
            .CircuitBreakerAsync(BreakOnExceptionsInRow, TimeSpan.FromSeconds(BrokenDuration));
    }
}