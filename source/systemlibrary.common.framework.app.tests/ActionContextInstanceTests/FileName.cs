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
                var options = new FrameworkServiceOptions();

                options.ApplicationParts = [
                    typeof(ActionContextInstanceTests).Assembly
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

                    Thread.Sleep(r.Next(1, 5));

                    var currentContext = HttpContextInstance.Current;

                    await context.Response.WriteAsync($"{queryString}=={currentContext?.Request.QueryString.Value}");

                });
            });
    }

    [TestMethod]
    public void High_Concurrency_Is_Thread_Safe()
    {
        var tasks = new Task<string>[10000];

        for (int i = 0; i < tasks.Length; i++)
        {
            var userName = $"User{i}";

            tasks[i] = Task.Run(async () =>
            {
                var sleep = Randomness.Int(0, 3);

                if (sleep > 0)
                    System.Threading.Thread.Sleep(sleep);

                return await GetResponseTextAsync($"?username={userName}");
            });
        }

        var results = Task.WhenAll(tasks).GetAwaiter().GetResult();

        for (int i = 0; i < results.Length; i++)
        {
            if (results[i] == null) continue;

            Assert.IsTrue(results[i].Contains("?username=User" + i + "==?username=User" + i), "Error at " + i + " result is " + results[i]);
        }
    }
}
