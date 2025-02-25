namespace SystemLibrary.Common.Framework.App;

partial class Client
{
    bool IsEligibleForRequestBreakerPolicy(RequestOptions options)
    {
        if (!UseRequestBreakerPolicy) return false;

        if (options.ContentType == ContentType.html ||
            options.ContentType == ContentType.javascript ||
            options.ContentType == ContentType.octetStream ||
            options.ContentType == ContentType.css) return false;

        return !options.Url.IsFile();
    }
}
