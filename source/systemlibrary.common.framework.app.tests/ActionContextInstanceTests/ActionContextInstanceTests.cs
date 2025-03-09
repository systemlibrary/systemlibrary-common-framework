using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.App.Extensions;
using SystemLibrary.Common.Framework.Tests;

namespace SystemLibrary.Common.Framework.App;

[TestClass]
public class ActionContextInstanceTests : BaseTest
{
    public ActionContextInstanceTests()
    {
        WebHostBuilder = new WebHostBuilder()
            .ConfigureServices(services =>
            {
                var options = new FrameworkOptions();

                options.ApplicationParts = [
                    typeof(ActionContextInstanceTests).Assembly
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

                    var sleep = Randomness.Int(0, 15);

                    if (sleep > 0)
                        Thread.Sleep(sleep);

                    var currentContext = ActionContextInstance.Current?.HttpContext;
                    // NOTE: It's always null, but at least we invoke it several times...
                    if (currentContext == null)
                        currentContext = HttpContextInstance.Current;

                    await context.Response.WriteAsync($"{queryString}=={currentContext.Request.QueryString.Value}");
                });
            });
    }

    [TestMethod]
    public async Task High_Concurrency_Is_Thread_Safe()
    {
        ThreadPool.SetMinThreads(40, 40);
        var tasks = new Task<string>[39];

        for (int i = 0; i < tasks.Length; i++)
        {
            var userName = $"User{i}";

            tasks[i] = Task.Run(async () =>
            {
                var sleep = Randomness.Int(0, 10);

                if (sleep > 0)
                    Thread.Sleep(sleep);

                return await GetResponseTextAsync($"?username={userName}");
            });
        }

        var results = await Task.WhenAll(tasks);

        var c = 0;
        for (int i = 0; i < results.Length; i++)
        {
            if (results[i] == null) continue;
            c++;
            Assert.IsTrue(results[i].Contains($"?username=User{i}==?username=User{i}"),
                          $"Error at {i}, result is {results[i]}");
        }

        Assert.IsTrue(c == tasks.Length, $"Too few {c}");
    }
}
