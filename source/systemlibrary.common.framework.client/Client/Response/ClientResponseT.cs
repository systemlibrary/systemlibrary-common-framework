using System;
using System.Net;
using System.Net.Http;
using System.Text.Json.Serialization;

namespace SystemLibrary.Common.Framework.App;

/// <summary>
/// The response that all Client methods returns
/// <para>Note that a response statuscode might be 200 OK, but the IsSuccess might be false in scenarios where no response were returned</para>
/// </summary>
/// <typeparam name="T">T can be a string that you later can convert to json, or it can be a class, or a list of a class that will automatically be converted from json, assuming json response</typeparam>
public partial class ClientResponse<T> : ClientResponse, IClientResponse, IDisposable
{
    internal ClientResponse(HttpResponseMessage response, T data, string reason = null)
    {
        Response = response;
        Data = data;
        Reason = reason;
        if (reason != null)
        {
            if (response == null)
            {
                if (reason.StartsWith("No such host"))
                    Code = (HttpStatusCode)599;
                else
                    Code = HttpStatusCode.BadGateway;
            }
        }
    }

    /// <summary>
    /// Optional: pass in a default data with response status code of 500 
    /// <para>Useful if you want to give some custom messages/data upwards to the callee</para>
    /// </summary>
    public ClientResponse(T defaultData = default)
    {
        Data = defaultData;
        Response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
        Response.ReasonPhrase = HttpStatusCode.InternalServerError.ToString();
    }

    /// <summary>
    /// Pass in a default data with response status code
    /// <para>Useful if you want to give some custom messages/data and custom status code upwards to the callee</para>
    /// </summary>
    public ClientResponse(T defaultData, HttpStatusCode statusCode)
    {
        Data = defaultData;
        Response = new HttpResponseMessage(statusCode);
        Response.ReasonPhrase = statusCode.ToString();
    }

    /// <summary>
    /// Pass in a custom status code of your own choice
    /// </summary>
    public ClientResponse(HttpStatusCode statusCode)
    {
        Data = default;
        Response = new HttpResponseMessage(statusCode);
        Response.ReasonPhrase = statusCode.ToString();
    }

    [JsonIgnore]
    string Reason;

    [JsonIgnore]
    HttpStatusCode Code = HttpStatusCode.OK;

    public new T Data { get; }

    object IClientResponse.Data => Data;

    public HttpStatusCode StatusCode => Response?.StatusCode ?? Code;
    public string Message => Response?.ReasonPhrase ?? Reason;

    public void Dispose()
    {
        if (Response != null)
        {
            try
            {
                Response.Dispose();
            }
            catch
            {
            }
        }
    }
}
