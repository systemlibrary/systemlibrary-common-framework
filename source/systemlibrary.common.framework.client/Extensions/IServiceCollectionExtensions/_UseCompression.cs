using System.IO.Compression;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;

namespace SystemLibrary.Common.Framework.App.Extensions;

static partial class IServiceCollectionExtensions
{
    static string[] MimeTypesToCompress = new string[]
    {
        "text/html",
        "text/html; charset=utf-8",
        "text/css",
        "text/css; charset=utf-8",
        "text/csv",
        "text/csv; charset=utf-8",
        "text/plain",
        "text/plain; charset=utf-8",
        "text/javascript",
        "text/javascript; charset=utf-8",
        "text/xml",

        "font/ttf",
        "font/otf",
        "font/woff2",
        "font/woff",

        "image/svg+xml",
        "image/webp",

        "application/json",
        "application/javascript",
        "application/rss+xml",
        "application/pkcs8",
        "application/xml",
    };

    static IServiceCollection UseBrotliCompression(this IServiceCollection services)
    {
        services.Configure<BrotliCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal);

        services.AddResponseCompression(compression =>
        {
            compression.EnableForHttps = true;
            compression.MimeTypes = MimeTypesToCompress;
            compression.Providers.Add<BrotliCompressionProvider>();
        });

        return services;
    }

    static IServiceCollection UseGzipCompression(this IServiceCollection services)
    {
        services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal);

        services.AddResponseCompression(compression =>
        {
            compression.EnableForHttps = true;
            compression.MimeTypes = MimeTypesToCompress;
            compression.Providers.Add<GzipCompressionProvider>();
        });

        return services;
    }
}
