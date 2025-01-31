using System;

using Prometheus;

namespace SystemLibrary.Common.Framework.App;

partial class Client
{
    static readonly Counter ClientRequestCounter = Metrics.CreateCounter(
        "client_requests_total",
        "Counts client requests by result",
        new CounterConfiguration
        {
            LabelNames = new[] { "uri", "status", "statusCode" }
        });

    string GetUriLabel(string typeName, Uri uri)
    {
        if (typeName == "Client")
        {
            typeName = uri.Host.ToLowerInvariant();
        }
        else
        {
            typeName = typeName.ToLowerInvariant();
        }

        string path = uri.AbsolutePath;

        if (path.Length <= 1)
        {
            return typeName.ToLowerInvariant();
        }

        if (path[^1] == '/')
        {
            path = path[..^1];
        }

        // Split path on '/' and take only the first three non-empty segments.
        int slashCount = 0;
        for (int i = 1; i < path.Length && slashCount < 3; i++)
        {
            if (path[i] == '/')
            {
                slashCount++;
                continue;
            }

            int end = path.IndexOf('/', i); 

            if (end == -1) end = path.Length;

            typeName += $"/{path[i..end].ToLowerInvariant()}";

            i = end - 1;
        }

        return typeName;
    }
}
