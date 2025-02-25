using System.Text.Json;

namespace SystemLibrary.Common.Framework.App;

/// <summary>
/// Client is a class for all http(s) requests in your project
/// <para>Uses HttpClient and Polly behind the scenes for features such as reusing tcp connections, retry on 502 and 504 status codes, and optionally a request breaker for 7 seconds, if 25 exceptions occurs in a row</para>
/// Request is always eligible for retry if request is a file request or GET or POST and status code is 502 or 504
/// <para>Options:</para>
/// useRetryPolicy:
/// <para> True: same as 'false', but adds:</para>
/// - retries one additional time on 502, 504 GET, POST
/// <para>- retries up to two times if response is null (timeout/no response)</para>
/// - retries once on 401 GET, POST
/// <para>- retries once on 404 GET</para>
/// - retries once on 500 GET, POST
/// <para>- retries once on 404, 500, 502 504 file request</para>
/// - retries once on OPTION, PATCH, HEAD, CONNECT, TRACE
/// </summary>
/// <remarks>
/// Every client uses the Default configurations, which you can override in appSettings.json
/// <para>Most default configurations can again be overriden in the Clients constructor</para>
/// Each HttpClient pool behind the scenes, is based on scheme, url, port and timeout
/// <para>Each method also take an additional timeout parameter, a different timeout will target a different HttpClient</para>
/// Each underlying HttpClient is used for default 20 minutes, each TCP connection is maximum reused for 4 minutes and 55 seconds
/// <para>A 502 or 504 response on GET, POST or file request will always be retried once, cannot be turned off unless RetryTimeout is set to 0 in appSettings</para>
/// A new client wont neccesary create a new HttpClient, it might use from cache, it all depends on the url and params you pass in
/// <para>- In theory you could create just one client reusing towards any url you want</para>
/// - But it's now up to you now: You want a new instance? Injection? Create your own static wrapper? Sure!
/// </remarks>
/// <example>
/// Configure the client in appSettings.json, heres the default:
/// "client": {
///   "timeout": 40001,
///   "retryTimeout": 10000, // The second retry will use half this duration, which is only used if useRetryPolicy is true
///   "ignoreSslErrors": true,
///   "useRetryPolicy": true, // one retry on 502 and 504 is GET/POST, cannot be turned off
///   "throwOnUnsuccessful": true,
///   "useRequestBreakerPolicy": false,
///   "clientCacheDuration": 1200
/// }
/// You can simply "var client = new Client()" if you want and use the methods, or you can inherit it on your integrations for reusing headers, api url, timeout configuration, etc...
/// 
/// A simple class and HttpBinClient:
/// <code>
/// class HttpBinResponse
/// {
///     public string Url { get; set; }
/// }
///</code>
///<code>
/// class HttpBinClient : Client
/// {
///     const string apiUrl = "http://httpbin.org";
///     
///     public HttpBinClient() : base(useRetryPolicy: true)
///     {
///     }
///     
///     public HttpBinResponse Get()
///     {
///         return base.Get&lt;HttpBinResponse&gt;(apiUrl + "/get").Data;
///     }
/// }
///</code>
///
/// Running the above Client and Response in a UnitTest project as such:
/// <code>
/// [TestMethod]
/// public void Test()
/// {
///     var client = new HttpBinClient();
/// 
///     var response = client.Get();
/// 
///     Assert.IsTrue(response.Url.Contains("http"));
///     //Visit: http://httpbin.org/get to see the actual value of 'url', then you know this Assert statement is true
/// }
/// </code>
/// 
/// Another example of using the Client directly:
/// <code>
/// public void Test()
/// {
///     var client = new Client();
/// 
///     var response = client.Get&lt;string&gt;("http://httpbin.org/get");
/// 
///     Assert.IsTrue(response.Contains("http"));
///     //Response is now the whole json (or any data actually as a string) text that the url: http://httpbin.org/get is returning
/// }
/// </code>
/// Another example returning HttpResponseMessage as is, for you to read the stream/content of the response yourself:
/// <code>
/// public void Test() 
/// {
///     var client = new Client();
///     var response = = client.Get&lt;HttpResponseMessage&gt;("http://httpbin.org/get");
///     var httpResponseMessage = response.Data;
///     // httpResponseMessage now is ready to be read if you need to read it manually, as in: it's not a json/xml/serialization type of response, but maybe an Stream/Image you need to read...
/// }
/// </code>
/// </example>
public partial class Client
{
    bool UseRetryPolicy;
    bool IgnoreSslErrors;
    bool ThrowOnUnsuccessful;
    int Timeout;
    int RetryTimeout;
    bool UseRequestBreakerPolicy;

    /// <summary>
    /// Create a new client
    /// </summary>
    /// <param name="useRetryPolicy">Will retry 404 once, and adds one more retry with half retry-timeout for 502 and 504</param>
    /// <param name="ignoreSslErrors">Ignore some common SSL errors that occurs, an expired or in-error SSL cert is still used to encrypt data</param>
    /// <param name="timeout">Override default timeout for this Client</param>
    /// <param name="retryTimeout">Override default retry timeout for this Client</param>
    /// <param name="throwOnUnsuccessful">Override default from appSettings</param>
    /// <param name="useRequestBreakerPolicy">Override default from appSettings</param>
    public Client(
        int? timeout = null,
        bool? useRetryPolicy = null,
        bool? ignoreSslErrors = null,
        int? retryTimeout = null,
        bool? useRequestBreakerPolicy = null,
        bool? throwOnUnsuccessful = null
        )
    {
        UseRetryPolicy = useRetryPolicy ?? UseRetryPolicyConfig;
        IgnoreSslErrors = ignoreSslErrors ?? IgnoreSslErrorsConfig;
        ThrowOnUnsuccessful = throwOnUnsuccessful ?? ThrowOnUnsuccessfulConfig;
        UseRequestBreakerPolicy = useRequestBreakerPolicy ?? UseRequestBreakerPolicyConfig;

        if (timeout != null)
            Timeout = timeout.Value;
        else
            Timeout = TimeoutConfig;

        if (retryTimeout != null)
            RetryTimeout = retryTimeout.Value;
        else
            RetryTimeout = RetryTimeoutConfig;
    }

    /// <summary>
    /// Send a HTTP GET request with a payload
    /// </summary>
    /// <example>
    /// <code>
    /// var data = new { hello: "world"};
    /// var response = new Client().Get&lt;string&gt;("https://www.systemlibrary.com/get", data, ContentType.json, 2000);
    /// </code>
    /// </example>
    public ClientResponse<T> Get<T>(string url, object payload, ContentType contentType = ContentType.Auto, IDictionary<string, string> headers = null, int timeoutMilliseconds = DefaultTimeout, JsonSerializerOptions jsonSerializerOptions = default, CancellationToken cancellationToken = default, Func<string, T> deserialize = null)
    {
        return GetAsync<T>(url, payload, contentType, headers, timeoutMilliseconds, jsonSerializerOptions, cancellationToken, deserialize)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    /// Send a HTTP GET request with a payload
    /// </summary>
    /// <example>
    /// <code>
    /// var data = new { hello: "world"};
    /// var response = new Client().Get&lt;string&gt;("https://www.systemlibrary.com/get", data, ContentType.json, 2000);
    /// </code>
    /// </example>
    public async Task<ClientResponse<T>> GetAsync<T>(string url, object payload, ContentType contentType = ContentType.Auto, IDictionary<string, string> headers = null, int timeoutMilliseconds = DefaultTimeout, JsonSerializerOptions jsonSerializerOptions = default, CancellationToken cancellationToken = default, Func<string, T> deserialize = null)
    {
        return await SendAsync<T>(HttpMethod.Get, url, payload, contentType, timeoutMilliseconds, headers, jsonSerializerOptions, cancellationToken, deserialize)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Send a HTTP GET request
    /// </summary>
    /// <example>
    /// <code>
    /// var response = new Client().Get&lt;string&gt;("https://www.systemlibrary.com/get", ContentType.json, 2000);
    /// </code>
    /// </example>
    public ClientResponse<T> Get<T>(string url, ContentType contentType = ContentType.Auto, IDictionary<string, string> headers = null, int timeoutMilliseconds = DefaultTimeout, JsonSerializerOptions jsonSerializerOptions = default, CancellationToken cancellationToken = default, Func<string, T> deserialize = null)
    {
        return GetAsync<T>(url, contentType, headers, timeoutMilliseconds, jsonSerializerOptions, cancellationToken, deserialize)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    /// Send a HTTP GET request
    /// </summary>
    /// <example>
    /// <code>
    /// var response = await new Client().GetAsync&lt;string&gt;("https://www.systemlibrary.com/get", ContentType.json, 2000);
    /// </code>
    /// </example>
    public async Task<ClientResponse<T>> GetAsync<T>(string url, ContentType contentType = ContentType.Auto, IDictionary<string, string> headers = null, int timeoutMilliseconds = DefaultTimeout, JsonSerializerOptions jsonSerializerOptions = default, CancellationToken cancellationToken = default, Func<string, T> deserialize = null)
    {
        return await SendAsync<T>(HttpMethod.Get, url, null, contentType, timeoutMilliseconds, headers, jsonSerializerOptions, cancellationToken, deserialize)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Send a HTTP HEAD request
    /// </summary>
    /// <example>
    /// <code>
    /// var response = new Client().Head&lt;string&gt;("https://www.systemlibrary.com/head", ContentType.json, 2000);
    /// </code>
    /// </example>
    public ClientResponse<T> Head<T>(string url, IDictionary<string, string> headers = null, int timeoutMilliseconds = DefaultTimeout, JsonSerializerOptions jsonSerializerOptions = default, CancellationToken cancellationToken = default, Func<string, T> deserialize = null)
    {
        return HeadAsync<T>(url, headers, timeoutMilliseconds, jsonSerializerOptions, cancellationToken, deserialize)
          .ConfigureAwait(false)
          .GetAwaiter()
          .GetResult();
    }

    /// <summary>
    /// Send a HTTP HEAD async request
    /// </summary>
    /// <example>
    /// <code>
    /// var response = await new Client().HeadAsync&lt;string&gt;("https://www.systemlibrary.com/head", ContentType.json, 2000);
    /// </code>
    /// </example>
    public async Task<ClientResponse<T>> HeadAsync<T>(string url, IDictionary<string, string> headers = null, int timeoutMilliseconds = DefaultTimeout, JsonSerializerOptions jsonSerializerOptions = default, CancellationToken cancellationToken = default, Func<string, T> deserialize = null)
    {
        return await SendAsync<T>(HttpMethod.Head, url, null, ContentType.None, timeoutMilliseconds, headers, jsonSerializerOptions, cancellationToken, deserialize)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Send a HTTP DELETE request
    /// </summary>
    /// <example>
    /// <code>
    /// var deleteId = 1;
    /// var response = new Client().Delete&lt;string&gt;("https://www.systemlibrary.com/delete", deleteId, ContentType.textplain, 2000);
    /// </code>
    /// </example>
    public ClientResponse<T> Delete<T>(string url, object data, ContentType contentType = ContentType.Auto, IDictionary<string, string> headers = null, int timeoutMilliseconds = DefaultTimeout, JsonSerializerOptions jsonSerializerOptions = default, CancellationToken cancellationToken = default, Func<string, T> deserialize = null)
    {
        return DeleteAsync<T>(url, data, contentType, headers, timeoutMilliseconds, jsonSerializerOptions, cancellationToken, deserialize)
          .ConfigureAwait(false)
          .GetAwaiter()
          .GetResult();
    }

    /// <summary>
    /// Send a HTTP DELETE async request
    /// </summary>
    /// <example>
    /// <code>
    /// var deleteId = 1;
    /// var response = await new Client().DeleteAsync&lt;string&gt;("https://www.systemlibrary.com/delete", deleteId, ContentType.textplain, 2000);
    /// </code>
    /// </example>
    public async Task<ClientResponse<T>> DeleteAsync<T>(string url, object data, ContentType contentType = ContentType.Auto, IDictionary<string, string> headers = null, int timeoutMilliseconds = DefaultTimeout, JsonSerializerOptions jsonSerializerOptions = default, CancellationToken cancellationToken = default, Func<string, T> deserialize = null)
    {
        return await SendAsync<T>(HttpMethod.Delete, url, data, contentType, timeoutMilliseconds, headers, jsonSerializerOptions, cancellationToken, deserialize)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Send a HTTP PUT request
    /// </summary>
    /// <example>
    /// <code>
    /// var putId = 1;
    /// var response = new Client().Put&lt;string&gt;("https://www.systemlibrary.com/put", putId, ContentType.textplain, 2000);
    /// </code>
    /// </example>
    public ClientResponse<T> Put<T>(string url, object data, ContentType contentType = ContentType.Auto, IDictionary<string, string> headers = null, int timeoutMilliseconds = DefaultTimeout, JsonSerializerOptions jsonSerializerOptions = default, CancellationToken cancellationToken = default, Func<string, T> deserialize = null)
    {
        return PutAsync<T>(url, data, contentType, headers, timeoutMilliseconds, jsonSerializerOptions, cancellationToken, deserialize)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    /// Send a HTTP PUT async request
    /// </summary>
    /// <example>
    /// <code>
    /// var putId = 1;
    /// var response = await new Client().PutAsync&lt;string&gt;("https://www.systemlibrary.com/put", putId, ContentType.textplain, 2000);
    /// </code>
    /// </example>
    public async Task<ClientResponse<T>> PutAsync<T>(string url, object data, ContentType contentType = ContentType.Auto, IDictionary<string, string> headers = null, int timeoutMilliseconds = DefaultTimeout, JsonSerializerOptions jsonSerializerOptions = default, CancellationToken cancellationToken = default, Func<string, T> deserialize = null)
    {
        return await SendAsync<T>(HttpMethod.Put, url, data, contentType, timeoutMilliseconds, headers, jsonSerializerOptions, cancellationToken, deserialize)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Send a HTTP POST request
    /// </summary>
    /// <example>
    /// <code>
    /// var postId = 1;
    /// var response = new Client().Post&lt;string&gt;("https://www.systemlibrary.com/post", postId, ContentType.textplain, 2000);
    /// </code>
    /// </example>
    public ClientResponse<T> Post<T>(string url, object data = null, ContentType contentType = ContentType.Auto, IDictionary<string, string> headers = null, int timeoutMilliseconds = DefaultTimeout, JsonSerializerOptions jsonSerializerOptions = default, CancellationToken cancellationToken = default, Func<string, T> deserialize = null)
    {
        return PostAsync<T>(url, data, contentType, headers, timeoutMilliseconds, jsonSerializerOptions, cancellationToken, deserialize)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    /// Send a HTTP POST async request
    /// </summary>
    /// <example>
    /// <code>
    /// var postId = 1;
    /// var response = await new Client().PostAsync&lt;string&gt;("https://www.systemlibrary.com/post", postId, ContentType.textplain, 2000);
    /// </code>
    /// </example>
    public async Task<ClientResponse<T>> PostAsync<T>(string url, object data, ContentType contentType = ContentType.Auto, IDictionary<string, string> headers = null, int timeoutMilliseconds = DefaultTimeout, JsonSerializerOptions jsonSerializerOptions = default, CancellationToken cancellationToken = default, Func<string, T> deserialize = null)
    {
        return await SendAsync<T>(HttpMethod.Post, url, data, contentType, timeoutMilliseconds, headers, jsonSerializerOptions, cancellationToken, deserialize)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Send a HTTP OPTIONS request
    /// </summary>
    /// <example>
    /// <code>
    /// var response = new Client().Options&lt;string&gt;("https://www.systemlibrary.com/options");
    /// </code>
    /// </example>
    public ClientResponse<T> Options<T>(string url, IDictionary<string, string> headers = null, int timeoutMilliseconds = DefaultTimeout, CancellationToken cancellationToken = default, Func<string, T> deserialize = null)
    {
        return OptionsAsync<T>(url, headers, timeoutMilliseconds, cancellationToken, deserialize)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    /// Send a HTTP OPTIONS request
    /// </summary>
    /// <example>
    /// <code>
    /// var response = await new Client().OptionsAsyncT&lt;string&gt;("https://www.systemlibrary.com/options");
    /// </code>
    /// </example>
    public async Task<ClientResponse<T>> OptionsAsync<T>(string url, IDictionary<string, string> headers = null, int timeoutMilliseconds = DefaultTimeout, CancellationToken cancellationToken = default, Func<string, T> deserialize = null)
    {
        return await SendAsync<T>(HttpMethod.Options, url, null, ContentType.None, timeoutMilliseconds, headers, default, cancellationToken, deserialize)
            .ConfigureAwait(false);
    }
}