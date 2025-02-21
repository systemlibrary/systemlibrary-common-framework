namespace SystemLibrary.Common.Framework.App;

/// <summary>
/// An enum of various media types that can be sent to the Client request methods
/// <para>The 'MediaType' is sent as an 'Accept' HEADER in the request</para>
/// <para>NOTE: Not all of them have been implemented yet though, but all will be sent as 'ACCEPT' header if specified, if you specify MediaType.none, no accept header is sent</para>
/// </summary>
/// <example>
/// <code>
///   var httpClient = new HttpBaseClient();
///   var response = httpClient.Post&lt;string&gt;("https://systemlibrary.com/post", data, MediaType.textplain);
/// </code>
/// </example>
public enum MediaType
{
    /// <summary>
    /// Sends data as application/json, if 'data' passed to the client through put/post/get already is a string, no conversion is made, else it is being converted to json string before data is sent
    /// </summary>
    [EnumValue("application/json")]
    json,

    /// <summary>
    /// Sends data as application/x-www-form-urlencoded
    /// </summary>
    [EnumValue("application/x-www-form-urlencoded")]
    xwwwformUrlEncoded,

    /// <summary>
    /// Sends data as text/plain, if 'data' passed to the client through put/post/get already is a string, no conversion is made, else it is being converted to json string before data is sent
    /// </summary>
    [EnumValue("text/plain")]
    plain,

    [EnumValue("multipart/form-data")]
    multipartFormData,

    [EnumValue("application/octet-stream")]
    octetStream,

    [EnumValue("text/html")]
    html,

    [EnumValue("text/css")]
    css,

    [EnumValue("text/javascript")]
    javascript,

    [EnumValue("application/pdf")]
    pdf,

    [EnumValue("application/zip")]
    zip,

    [EnumValue("text/xml")]
    xml,

    [EnumValue("application/graphql")]
    graphql,

    [EnumValue("json-patch+json")]
    jsonPatch,
    

    [EnumValue("")]
    None
}
