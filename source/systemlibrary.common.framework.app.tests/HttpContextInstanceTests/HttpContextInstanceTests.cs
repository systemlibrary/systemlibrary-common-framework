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
                var options = new FrameworkOptions();

                options.ApplicationParts = [
                    typeof(HttpContextInstanceTests).Assembly
                ];

                services = services.AddFrameworkServices<LogWriter>(options);
            })
            .Configure(app =>
            {
                var options = new FrameworkOptions();

                options.UseHttpsRedirection = false;

                app.UseFrameworkMiddlewares(null, options);

                app.Run(async context =>
                {
                    var queryString = context.Request.QueryString.Value;

                    var sleep = Randomness.Int(0, 100);

                    if (sleep > 0)
                        Task.Delay(sleep);

                    var currentContext = HttpContextInstance.Current;

                    await context.Response.WriteAsync($"{queryString}|{currentContext?.Request.QueryString.Value}");
                });
            });
    }

    [TestMethod]
    public async Task HttpContextInstance_Current_IsThreadSafe_PerRequest()
    {
        ThreadPool.SetMinThreads(40, 40);
        var tasks = new Task<string>[39];

        var r = new Random();
        for (int i = 0; i < tasks.Length; i++)
        {
            var userName = $"User{i}";

            tasks[i] = Task.Run(async () =>
            {
                var sleep = Randomness.Int(0, 10);

                if (sleep > 0)
                    Thread.Sleep(sleep);

                var response = await Client.GetAsync($"?username={userName}");

                return await response.Content.ReadAsStringAsync();
            });
        }

        var results = await Task.WhenAll(tasks);

        var c = 0;
        for (int i = 0; i < results.Length; i++)
        {
            if (results[i] == null) continue;
            c++;
            Assert.IsTrue(results[i].Contains("?username=User" + i + "|?username=User" + i), "Error at " + i + " result is " + results[i] + " Expected " + "?username=User" + i + "|?username=User" + i);
        }
        Assert.IsTrue(results.Length == c, "Too few " + c + " vs " + results.Length);
        Assert.IsTrue(c > 10 && c == tasks.Length, "Too few: " + c);
    }
}