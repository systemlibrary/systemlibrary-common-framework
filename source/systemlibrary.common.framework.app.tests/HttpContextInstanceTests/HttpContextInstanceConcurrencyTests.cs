using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SystemLibrary.Common.Framework.App;

[TestClass]
public class HttpContextInstanceConcurrencyTests
{
    private TestServer _server;
    private HttpClient _client;

    [TestInitialize]
    public void Setup()
    {
        //_server = new TestServer(new WebHostBuilder()
        //    .ConfigureServices(services =>
        //    {
        //        services.AddCommonWebServices();
        //    })
        //    .Configure(app =>
        //    {
        //        var options = new AppBuilderOptions();
        //        options.UseHsts = false;
        //        options.UseHttpsRedirection = false;
        //        app.UseCommonWebApp(null, options);

        //        app.Run(async context =>
        //        {
        //            var r = new Random();

        //            System.Threading.Thread.Sleep(r.Next(0, maxValue: 25));
        //            var username = context.Request.Query["username"];

        //            System.Threading.Thread.Sleep(r.Next(0, maxValue: 25));

        //            var currentContext = HttpContextInstance.Current;
        //            System.Threading.Thread.Sleep(r.Next(0, maxValue: 25));

        //            var hash = r.Next(1, 5).ToString().ToMD5Hash();

        //            await context.Response.WriteAsync($"{username}|{currentContext?.Request.Query["username"]}" + hash);
        //        });
        //    }));

        //_client = _server.CreateClient();
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
                    System.Threading.Thread.Sleep(r.Next(1, 10));
                    var response = await _client.GetAsync($"?username={userName}");

                    return await response.Content.ReadAsStringAsync();
                }
                catch (Exception)
                {
                    return default;
                }
            });
        }

        var results = await Task.WhenAll(tasks);

        for (int i = 0; i < results.Length; i++)
        {
            Assert.IsTrue(results[i].Contains("User" + i + "|" + "User" + i), "Error at " + i + " result is " + results[i]);
        }
    }

}