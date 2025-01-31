using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.Tests;

namespace SystemLibrary.Common.Framework;

[TestClass]
public class AsyncTests : BaseTest
{
    [TestMethod]
    public void Run_Return_String_Async_Added_To_List_Returns_All()
    {
        var result = Async.Run(
            () => OnAsyncRun("Hello world"),
            () => OnAsyncRun("Hello world"),
            () => OnAsyncRun("Hello world"),
            () => OnAsyncRun("Hello world")
        );

        var sum = result.Sum();

        Assert.IsTrue(sum == 4, "Async.Run did not execute all " + sum);
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
        var path2 = "C:\\syslib\\systemlibrary-common-framework\\source\\systemlibrary.common.framework.net.tests\\config\\";
        var path3 = "C:\\syslib\\systemlibrary-common-framework\\source\\systemlibrary.common.framework.net.tests\\Configs\\";
        var path4 = "C:\\syslib\\systemlibrary-common-framework\\source\\systemlibrary.common.framework.net.tests\\configuration\\";
        var path5 = "C:\\syslib\\systemlibrary-common-framework\\source\\systemlibrary.common.framework.net.tests\\Configurations\\";
        var files = Async.Run(
            () => ConfigFileSearcher.GetConfigurationFilesInFolder(path1, false),
            () => ConfigFileSearcher.GetConfigurationFilesInFolder(path2, true),
            () => ConfigFileSearcher.GetConfigurationFilesInFolder(path3, true),
            () => ConfigFileSearcher.GetConfigurationFilesInFolder(path4, true),
            () => ConfigFileSearcher.GetConfigurationFilesInFolder(path5, true)
        );

        Assert.IsTrue(files.Count == 5, "One or more directory searches failed: " + files.Count );

        var any = false;
        foreach(var configs in files)
        {
            if (configs.Length == 6)
                any = true;
        }

        Assert.IsTrue(any, "One or more directory searches should return 6 config files, but didnt, so some config files missing");
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