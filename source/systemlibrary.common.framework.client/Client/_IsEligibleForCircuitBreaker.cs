namespace SystemLibrary.Common.Framework.App;

partial class Client
{
    bool IsEligibleForRequestBreakerPolicy(RequestOptions options)
    {
        if (!UseRequestBreakerPolicy) return false;

        if (options.MediaType == MediaType.html ||
            options.MediaType == MediaType.javascript ||
            options.MediaType == MediaType.octetStream ||
            options.MediaType == MediaType.css) return false;

        return !options.Url.IsFile();
    }
}
