using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.App.Extensions;
using SystemLibrary.Common.Framework.Tests;

namespace SystemLibrary.Common.Framework.App;

[TestClass]
public class HttpContextInstanceTests : BaseTest
{
    public HttpContextInstanceTests()
    {
        WebHostBuilder = new WebHostBuilder()
            .ConfigureServices(services =>
            {
                var options = new FrameworkServiceOptions();

                options.ApplicationParts = [
                    typeof(HttpContextInstanceTests).Assembly
                ];

                services = services.AddFrameworkServices<LogWriter>(options);
            })
            .Configure(app =>
            {
                var options = new FrameworkAppOptions();

                options.UseHsts = false;
                options.UseHttpsRedirection = false;

                app.UseFrameworkMiddlewares(null, options);

                app.Run(async context =>
                {
                    var queryString = context.Request.QueryString.Value;

                    var r = new Random();

                    Thread.Sleep(r.Next(1, 15));

                    var currentContext = HttpContextInstance.Current;

                    await context.Response.WriteAsync($"{queryString}|{currentContext?.Request.QueryString.Value}");
                });
            });
    }

    [TestMethod]
    public async Task HttpContextInstance_Current_IsThreadSafe_PerRequest()
    {
        var tasks = new Task<string>[10000];

        var r = new Random();
        for (int i = 0; i < tasks.Length; i++)
        {
            var userName = $"User{i}";

            tasks[i] = Task.Run(async () =>
            {
                try
                {
                    var sleep = Randomness.Int(0, 7);

                    if (sleep > 0)
                        Thread.Sleep(sleep);

                    var response = await Client.GetAsync($"?username={userName}");

                    return await response.Content.ReadAsStringAsync();
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                    return default;
                }
            });
        }

        var results = await Task.WhenAll(tasks);

        var c = 0;
        for (int i = 0; i < results.Length; i++)
        {
            if (results[i] == null) continue;
            c++;
            Assert.IsTrue(results[i].Contains("?username=User" + i + "|?username=User" + i), "Error at " + i + " result is " + results[i]);
        }
        Assert.IsTrue(results.Length == c, "Too few " + c + " vs " + results.Length);
    }
}