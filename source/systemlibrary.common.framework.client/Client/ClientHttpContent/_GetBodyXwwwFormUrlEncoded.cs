using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Http;
using System.Text;

using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework.App;

partial class Client
{
    partial class ClientHttpContent
    {
        static HttpContent GetBodyXwwwFormUrlEncoded(object data)
        {
            if (data is List<KeyValuePair<string, string>> keyValuePairCollection)
            {
                return new FormUrlEncodedContent(keyValuePairCollection);
            }
            else if (data is IDictionary dictionary)
            {
                var keyValuePairs = new List<KeyValuePair<string, string>>();

                foreach (DictionaryEntry keyValue in dictionary)
                    keyValuePairs.Add(new KeyValuePair<string, string>(keyValue.Key.ToString(), keyValue.Value?.ToString()));

                return new FormUrlEncodedContent(keyValuePairs);
            }
            else if (data is ExpandoObject expando)
            {
                throw new Exception("Expando is currently not fully implemented in GetBodyXwwwFormUrlEncoded()");
            }
            else if (data is string text)
            {
                if (text.Contains("&") && text.Contains("="))
                {
                    var keyValues = new List<KeyValuePair<string, string>>();

                    var inputs = text.Split('&');
                    foreach (var input in inputs)
                    {
                        if (input.IsNot()) continue;

                        var keyValue = input.Split('=');

                        if (keyValue.IsNot()) continue;

                        keyValues.Add(new KeyValuePair<string, string>(keyValue[0], keyValue.Length > 1 ? keyValue[1] : null));
                    }

                    return new FormUrlEncodedContent(keyValues);
                }
                return new StringContent(text, Encoding.UTF8);
            }
            else if (data is byte[] bytes)
            {
                return new ByteArrayContent(bytes, 0, bytes.Length);
            }
            else 
            {
                var dataType = data.GetType();
                if (dataType.IsClass)
                {
                    var properties = dataType.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.GetProperty);

                    var formProperties = new Dictionary<string, string>();

                    if (properties?.Length > 0)
                    {
                        foreach (var property in properties)
                        {
                            if (property.PropertyType.IsListOrArray() || property.PropertyType.IsGenericType)
                                throw new Exception("Class has a property " + property.Name + " which is generic, or a list or an array. Not yet implemented in combination with wwwformUrlEncoded. Convert the class and its properties yourself to a Dictionary<string, string>() and send that as the data");

                            var value = property.GetValue(data);

                            if (value != null)
                                formProperties.Add(property.Name, value.ToString());
                        }
                    }

                    if (formProperties.Count == 0)
                        throw new Exception("Class without properties to wwwformurlencoded string is not currently fully implemented in GetBodyXwwwFormUrlEncoded(). Either your class is invalid or contains 0 properties, and properties must be 'public get'.");

                    return new FormUrlEncodedContent(formProperties);
                }
            }

            throw new Exception("x-www-form-urlencoded media type requires data sent to be either: List<KeyValuePair<string, string>> or IDictionary, or a string or a byte[] or a class with public properties, or lastly, but not recommended: Null, which requires the URL to already contain key/values as query string");
        }
    }
}