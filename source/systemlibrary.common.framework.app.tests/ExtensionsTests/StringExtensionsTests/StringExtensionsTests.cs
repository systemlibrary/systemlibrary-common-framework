using Microsoft.AspNetCore.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.App.Extensions;
using SystemLibrary.Common.Framework.Tests;

namespace SystemLibrary.Common.Framework.App;

[TestClass]
public class StringExtensionsTests : BaseTest
{
    const string DumpFullPath = "C:\\Logs\\systemlibrary-common-framework-app-tests.log";

    static string ReadFile()
    {
        try
        {
            Thread.Sleep(12);
            return File.ReadAllText(DumpFullPath);
        }
        catch
        {
            Thread.Sleep(12);
            return File.ReadAllText(DumpFullPath);
        }
    }

    void CreateWebHostBuilder()
    {
        var options = new FrameworkOptions
        {
            EncKeyDir = "C:\\temp\\"
        };

        WebHostBuilder = new WebHostBuilder()
            .ConfigureServices(services =>
            {
                services = services.AddFrameworkServices<LogWriter>(options);
            })
            .Configure(app =>
            {
                options.UseHttpsRedirection = false;

                app.UseFrameworkMiddlewares(null, options);

                MapActions(app);
            });
    }

    [TestMethod]
    public void FrameworkKeyDirectory_Is_Set_Encrypts_With_Partial_FileName_IsOk()
    {
        CreateWebHostBuilder();

        var text = GetResponseText("/" + nameof(FrameworkKeyDirectory_Is_Set_Encrypts_With_Partial_FileName_IsOk_Action));

        IsOk(text == "Hello world", "Got " + text);

        var log = ReadFile();

        Assert.IsTrue(log.Contains("found file in"), "key not from key file, or it sometimes errors as we run all tests (multiple 'applications'), they fight over one log file..." + log);
    }

    public string FrameworkKeyDirectory_Is_Set_Encrypts_With_Partial_FileName_IsOk_Action()
    {
        var data = "Hello world";

        var enc = data.Encrypt();
        var dec = enc.Decrypt();

        return dec;
    }
}


