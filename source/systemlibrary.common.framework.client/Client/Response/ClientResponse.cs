using System.Text.Json.Serialization;

namespace SystemLibrary.Common.Framework.App;

/// <summary>
/// Base class of a ClientResponse 
/// <para>- Contains the HttpResponseMessage itself </para>
/// Used when you do not want to return 'object' nor generic type, but you want to be clear in what object is returned in your C# functions
/// <para>- Contains 'data' variable for serializing purposes, so 'data' variable is never undefined in JS world (depends on the JSON serialization options youve set, but out of the box its null if not set)</para>
/// </summary>
public class ClientResponse
{
    [JsonIgnore]
    public HttpResponseMessage Response { get; internal set; }

    public bool IsSuccess => Response?.IsSuccessStatusCode == true;

    /// <remarks>
    /// Note: This variable is null or the 'data' variable from the generic ClientResponse
    /// <para>This variable is defined here too, to avoid undefined scenarios in JS world</para>
    /// <para>And this class is then used as base class for all responses so you can always cast anything to this class and then do a serialize that just gives the data without all other statuscode, httpmessage etc.</para>
    /// </remarks>
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