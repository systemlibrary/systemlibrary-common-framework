using Microsoft.AspNetCore.Mvc.Formatters;

namespace SystemLibrary.Common.Framework.App;

internal class DefaultSupportedMediaTypes : StringOutputFormatter
{
    internal DefaultSupportedMediaTypes()
    {
        SupportedMediaTypes.Add("audio/mpeg");
        SupportedMediaTypes.Add("audio/wave");
        SupportedMediaTypes.Add("audio/wav");
        SupportedMediaTypes.Add("audio/webm");
        SupportedMediaTypes.Add("audio/ogg");
        SupportedMediaTypes.Add("audio/x-ms-wma");

        SupportedMediaTypes.Add("application/x-font-ttf");
        SupportedMediaTypes.Add("application/x-font-opentype");

        SupportedMediaTypes.Add("application/octet-stream");
        SupportedMediaTypes.Add("application/zip");
        SupportedMediaTypes.Add("application/gzip");
        SupportedMediaTypes.Add("application/pdf");
        SupportedMediaTypes.Add("application/json");
        SupportedMediaTypes.Add("application/pkcs8");
        SupportedMediaTypes.Add("application/rss+xml");
        SupportedMediaTypes.Add("application/xml");
        SupportedMediaTypes.Add("application/javascript");
        SupportedMediaTypes.Add("application/atom+xml");
        SupportedMediaTypes.Add("application/xhtml+xml");

        SupportedMediaTypes.Add("font/ttf");
        SupportedMediaTypes.Add("font/otf");
        SupportedMediaTypes.Add("font/woff2");
        SupportedMediaTypes.Add("font/woff");
        SupportedMediaTypes.Add("font/eot");

        SupportedMediaTypes.Add("multipart/form-data");
        SupportedMediaTypes.Add("multipart/byteranges");

        SupportedMediaTypes.Add("image/svg+xml");
        SupportedMediaTypes.Add("image/apng");
        SupportedMediaTypes.Add("image/bmp");
        SupportedMediaTypes.Add("image/jpeg");
        SupportedMediaTypes.Add("image/gif");
        SupportedMediaTypes.Add("image/png");
        SupportedMediaTypes.Add("image/tiff");
        SupportedMediaTypes.Add("image/webp");
        SupportedMediaTypes.Add("image/x-icon");
        SupportedMediaTypes.Add("image/avif");

        SupportedMediaTypes.Add("text/html");
        SupportedMediaTypes.Add("text/css");
        SupportedMediaTypes.Add("text/csv");
        SupportedMediaTypes.Add("text/plain");
        SupportedMediaTypes.Add("text/javascript");
        SupportedMediaTypes.Add("text/xml");

        SupportedMediaTypes.Add("video/mp4");
        SupportedMediaTypes.Add("video/m4v");
        SupportedMediaTypes.Add("video/webm");
        SupportedMediaTypes.Add("video/ogg");
    }
}
