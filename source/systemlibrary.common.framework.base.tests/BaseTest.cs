﻿using System.Reflection;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;

namespace SystemLibrary.Common.Framework.Tests;

public abstract partial class BaseTest
{
    static object ClientLock = new object();

    TestServer Server;
    HttpClient _Client;

    protected IWebHostBuilder WebHostBuilder;

    protected HttpClient Client
    {
        get
        {
            if (_Client == null)
            {
                lock (ClientLock)
                {
                    if (_Client != null) return Client;

                    var appSettingsPath = AppContext.BaseDirectory + "appSettings.json";

                    if (File.Exists(appSettingsPath))
                    {
                        var configuration = new ConfigurationBuilder().AddJsonFile(appSettingsPath, optional: false, reloadOnChange: false).Build();

                        WebHostBuilder = WebHostBuilder.UseConfiguration(configuration);
                    }

                    Server = new TestServer(WebHostBuilder);

                    _Client = Server.CreateClient();
                }
            }
            return _Client;
        }
    }

    static MethodInfo[] Methods;
    protected void MapTestActions(IApplicationBuilder app)
    {
        if (Methods == null)
            Methods = this.GetType().GetMethods();

        foreach (var method in Methods)
        {
            var methodName = method.Name;

            if (!methodName.EndsWith("_Action")) continue;

            app.Map("/" + methodName, runner =>
            {
                runner.Run(async context =>
                {
                    var json = "No result";

                    try
                    {
                        json = (string)method.Invoke(this, null);
                    }
                    catch (Exception ex)
                    {
                        json = ex.ToString();

                        Log.Error(ex);
                    }

                    context.Response.ContentType = "application/json";

                    await context.Response.WriteAsync(json ?? "");
                });
            });
        }
    }
}

