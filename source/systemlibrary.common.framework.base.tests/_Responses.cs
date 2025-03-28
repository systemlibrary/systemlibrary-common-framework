﻿using System.Text;

namespace SystemLibrary.Common.Framework.Tests;

partial class BaseTest
{
    protected HttpResponseMessage GetResponseMessage(string path, params (string Key, string Value)[] headers)
    {
        var response = GetResponse(path, headers);

        return response;
    }

    protected string GetResponseText(string path, params (string Key, string Value)[] headers)
    {
        var response = GetResponse(path, headers);

        if (!response.IsSuccessStatusCode)
            Log.Dump("Not successful: " + path + " " + response.StatusCode + " " + response.ReasonPhrase);

        return response.Content.ReadAsStringAsync()
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();
    }

    protected async Task<string> GetResponseTextAsync(string path, params (string Key, string Value)[] headers)
    {
        var response = await GetResponseAsync(path, headers);

        if (!response.IsSuccessStatusCode)
            Log.Dump("Not successful: " + path + " " + response.StatusCode);

        return await response.Content.ReadAsStringAsync();
    }

    HttpResponseMessage GetResponse(string path, (string Key, string Value)[] headers)
    {
        return GetResponseAsync(path, headers)
            .GetAwaiter()
            .GetResult();
    }

    async Task<HttpResponseMessage> GetResponseAsync(string path, (string Key, string Value)[] headers)
    {
        if (!path.StartsWith("/"))
        {
            path = "/" + path;
        }

        var request = new HttpRequestMessage(HttpMethod.Get, path);

        if (headers != null)
        {
            foreach (var (key, value) in headers)
            {
                if (key.Is() && value != null)
                {
                    if (key == "Content-Type")
                    {
                        request.Content = new StringContent("", Encoding.UTF8, value);
                    }
                    request.Headers.TryAddWithoutValidation(key, value);
                }
            }
        }

        return await Client.SendAsync(request);
    }
}
