using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.Tests;

namespace SystemLibrary.Common.Framework;

[TestClass]
public class AsyncTests : BaseTest
{
    [TestMethod]
    public void Run_Return_String_Async_Added_To_List_Returns_All()
    {
        var result = Async.Tasks(
            () => OnAsyncRun("Hello world"),
            () => OnAsyncRun("Hello world"),
            () => OnAsyncRun("Hello world"),
            () => OnAsyncRun("Hello world")
        );

        var sum = result.Sum();

        Assert.IsTrue(sum == 4, "Async.Run did not execute all 4, sum is: " + sum);
    }

    [TestMethod]
    public void FireAndForget_Runs_Once_Success()
    {
        var file = @"C:\temp\asynctests" + DateTime.Now.ToString("fff") + ".txt";

        Async.FireAndForget(actions: () => File.AppendAllText(file, "Hello world"));

        Thread.Sleep(75);

        var text = File.ReadAllText(file);
        Assert.IsTrue(text.Contains("Hello world"));

        File.Delete(file);
    }

    [TestMethod]
    public void Run_Searches_For_Files_Async_Returns_All_Files_Successfully()
    {
        var path1 = "C:\\syslib\\systemlibrary-common-framework\\source\\systemlibrary.common.framework.net.tests";
        var path2 = "C:\\syslib\\systemlibrary-common-framework\\source\\systemlibrary.common.framework.net.tests\\Configs\\";
        var path3 = "C:\\syslib\\systemlibrary-common-framework\\source\\systemlibrary.common.framework.net.tests\\Configurations\\";
        var files = Async.Tasks(
            () => ConfigFileSearcher.GetConfigurationFilesInFolder(path1, false),
            () => ConfigFileSearcher.GetConfigurationFilesInFolder(path2, true),
            () => ConfigFileSearcher.GetConfigurationFilesInFolder(path3, true)
        );

        Assert.IsTrue(files.Count == 3, "Wrong config count: " + files.Count );

        var any = false;
        foreach(var configs in files)
        {
            if (configs.Length == 6)
                any = true;
        }

        Assert.IsTrue(any, "One config dir used to have 6 config files. Have you copied all config files to correct folders? Are there 6 json files in either of the config folders?");
    }

    [TestMethod]
    public void FireAndForget_Runs_Multiple_Success()
    {
        Async.FireAndForget(OnExceptionCallback, Call);
        Async.FireAndForget(OnExceptionCallback, Call);
        Async.FireAndForget(OnExceptionCallback, Call);

        Async.FireAndForget(OnExceptionCallback, Call);
        Async.FireAndForget(OnExceptionCallback, Call);
        Async.FireAndForget(OnExceptionCallback, Call);

        Async.FireAndForget(OnExceptionCallback, Call);
        Async.FireAndForget(OnExceptionCallback, Call);
        Async.FireAndForget(OnExceptionCallback, Call);

        Async.FireAndForget(OnExceptionCallback, Call);
        Async.FireAndForget(OnExceptionCallback, Call);
        Async.FireAndForget(OnExceptionCallback, Call);

        Thread.Sleep(1750);

        Assert.IsTrue(ExceptionCounter > 9, "Exception counter was: " + ExceptionCounter);
    }

    [TestMethod]
    public void Paralel_Reads_Configuration_Value_Success()
    {
        string Call(int i)
        {
            Thread.Sleep(10);
            var key = i.ToString();
            var value = Configs.AppSettings.Current.Child.Color;
            return key + "=" + value;
        }
        var results = Async.Parallel(
            () => Call(1),
            () => Call(2),
            () => Call(3),
            () => Call(4)
        );

        Assert.IsTrue(results.Count == 4, "Wrong count: " + results.Count);
        Assert.IsTrue(results[1].Contains("=red"), "Wrong: " + results[1] );
    }

    [TestMethod]
    public void Tasks_With_Timeout_Times_Out()
    {
        string Call(int i)
        {
            Thread.Sleep(500);
            var key = i.ToString();
            var value = Configs.AppSettings.Current.Child.Color;
            return key + "=" + value;
        }
        var results = Async.Tasks(400,
            () => Call(1),
            () => Call(2),
            () => Call(3),
            () => Call(4)
        );

        Assert.IsTrue(results.Count == 0, "Results returned items, it should return 0 as all times out " + results.Count);
    }

    [TestMethod]
    public void Tasks_Multiple_Async_Calls_Success()
    {
        string Call(int i)
        {
            var key = i.ToString();
            var value = Configs.AppSettings.Current.Child.Color;
            return key + "=" + value;
        }

        var results = Async.Tasks(
            () => Call(1),
            () => Call(2),
            () => Call(3),
            () => Call(4),
            () => Call(5),
            () => Call(6),
            () => Call(7),
            () => Call(8));

        foreach (var result in results)
        {
            var parts = result.Split('=');
            var red = parts[1];

            Assert.IsTrue(red == "red", "Missing red in " + parts[0]);
        }
    }

    static void Call()
    {
        var handler = new HttpClientHandler();
        handler.ClientCertificateOptions = ClientCertificateOption.Manual;
        handler.ServerCertificateCustomValidationCallback =
            (httpRequestMessage, cert, cetChain, policyErrors) =>
            {
                return true;
            };

        using (var client = new HttpClient(handler))
        {
            client.BaseAddress = new Uri("https://www.systemlibrary.com/unknown-url");
            client.Timeout = TimeSpan.FromMilliseconds(1500);
            var response = client.GetStringAsync("")
                .ConfigureAwait(true)
                .GetAwaiter()
                .GetResult();
        }
    }

    static int ExceptionCounter = 0;

    static void OnExceptionCallback(Exception ex)
    {
        ExceptionCounter++;
    }

    static int OnAsyncRun(string input)
    {
        if (input == null) return 0;

        return 1;
    }
}