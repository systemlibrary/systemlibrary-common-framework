using System.Text.Json;

namespace SystemLibrary.Common.Framework.App;

partial class Client
{
    internal class RequestOptions
    {
        public HttpMethod Method;
        public string Url;
        public Uri Uri;
        public string UriLabel;
        public JsonSerializerOptions JsonSerializerOptions;
        public object Data;
        public int Timeout;
        public int RetryTimeout;
        public IDictionary<string, string> Headers;
        public bool ForceNewClient;
        public bool UseRetryPolicy;
        public bool IgnoreSslErrors;
        public bool ExpectContinue;
        public bool UseAutomaticDecompression;
        public CancellationToken CancellationToken;
        public ContentType ContentType;
        public int RetryIndex;

        internal void Update(int retry)
        {
            if (retry == 1)
            {
                ForceNewClient = true;
            }
            else if (retry == 2)
            {
                ForceNewClient = true;
            }
            RetryIndex = retry;
        }

        public int GetTimeout()
        {
            if (RetryIndex == 1)
                return RetryTimeout;

            if (RetryIndex == 2)
                return int.Max(RetryTimeout / 2, 3000);

            return Timeout;
        }
    }
}
