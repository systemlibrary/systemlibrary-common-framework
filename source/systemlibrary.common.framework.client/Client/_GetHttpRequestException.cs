using System.Text;

using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework.App;

partial class Client
{
    static string GetRequestErrorMessage(RequestOptions options)
    {
        var contentTypeValue = options.ContentType.ToValue();
        if (contentTypeValue.IsNot())
            contentTypeValue = options.ContentType.ToString();

        return $"{options.Method} {options.Url} as {contentTypeValue} with timeout: {options.Timeout}ms.";
    }

    static HttpRequestException GetHttpRequestException(RequestOptions options, HttpResponseMessage response = null, Exception ex = null)
    {
        StringBuilder message = null;
        if (response == null)
        {
            message = new StringBuilder(GetRequestErrorMessage(options), 512);
        }
        else
        {
            message = new StringBuilder($"{(int)response.StatusCode} " + GetRequestErrorMessage(options), 512);
        }

        if (ex != null)
        {
            if (ex is TaskCanceledException tce)
            {
                if (tce.Message.Contains("task was"))
                    message.Append("has timed out or was canceled: ");
                else if (tce.Message.Contains("operation was"))
                    message.Append("operation was stopped, firewall or timeout: ");
            }
            else if (ex is HttpRequestException hre)
            {
                message.Append(hre.Message);
                ex = null;
            }
            else
            {
                message.Append("has invalid response: ");
            }

            if (ex != null)
                message.Append(ex.Message);
        }

        if (response != null)
        {
            if (response.Content == null)
            {
                message.Append($". Reason: {response.ReasonPhrase}");
            }
            else
            {
                try
                {
                    message.Append($". {response.ReasonPhrase} Reason: " + ReadResponseBodyAsStringAsync(response).ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult());
                }
                catch
                {
                    // Swallow
                    message.Append($". Reason: {response.ReasonPhrase}");
                }
            }
        }

        return new HttpRequestException(message.ToString(), ex, response?.StatusCode ?? (ex as HttpRequestException)?.StatusCode);
    }
}