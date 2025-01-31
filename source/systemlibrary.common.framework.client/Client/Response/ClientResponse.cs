using System.Net.Http;
using System.Text.Json.Serialization;

namespace SystemLibrary.Common.Framework.App;

/// <summary>
/// Base class of a ClientResponse 
/// <para>- Contains the HttpResponseMessage itself </para>
/// Used when you do not want to return 'object' nor generic type, but you want to be clear in what object is returned in your C# functions
/// <para>- Contains 'data' variable for serializing purposes so it is not undefined</para>
/// </summary>
public class ClientResponse
{
    [JsonIgnore]
    public HttpResponseMessage Response { get; internal set; }

    public bool IsSuccess => Response?.IsSuccessStatusCode == true;

    /// <summary>
    /// NOTE: Variable is always null, it exists so serializing data will not give 'undefined' in javascript-world
    /// </summary>
    public object Data
    {
        get
        {
            if (this is IClientResponse i)
            {
                return i.Data;
            }

            return null;
        }
    }
}