using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.App.Extensions;
using SystemLibrary.Common.Framework.Tests;

namespace SystemLibrary.Common.Framework.App;

// NOTE: BaseTest registers only first WebHostBuilder and its reused, so each test here should be ran manually
// TODO: Update so each WebHostBuilder is attached to its "own function" and "function name" so it runs in a "sharded way"
// Might have to killeach webhostbuilder after complete before next one executes as it might reuse the port and localhost domain name internally
[TestClass]
public class IServiceCollectionExtensionsTests : BaseTest
{
    FrameworkOptions FrameworkOptions = new FrameworkOptions();

    void DisableFrameworkOptions()
    {
        FrameworkOptions.UseBrotliResponseCompression = false;
        FrameworkOptions.UseGzipResponseCompression = false;
        FrameworkOptions.UseMvc = false;
        FrameworkOptions.UseHttpsRedirection = false;
        FrameworkOptions.UseControllers = false;
        FrameworkOptions.UseCookiePolicy = false;
        FrameworkOptions.UseForwardILogger = false;
        FrameworkOptions.UseDataProtectionPolicy = false;
        FrameworkOptions.UseExtendedEnumModelConverter = false;
        FrameworkOptions.UseForwardedHeaders = false;
        FrameworkOptions.UseResponseCaching = false;
        FrameworkOptions.UseRazorRuntimeCompilationOnSave = false;
        FrameworkOptions.UseStaticFilePolicy = false;
        FrameworkOptions.UseAuthentication = false;
        FrameworkOptions.UseAuthorization = false;
        FrameworkOptions.UseDeveloperPage = false;
        FrameworkOptions.UseOutputCache = false;
    }

    void CreateWebHostBuilder()
    {
        WebHostBuilder = new WebHostBuilder()
            .ConfigureServices(services =>
            {
                services = services.AddFrameworkServices<LogWriter>(FrameworkOptions);
            })
            .Configure(app =>
            {
                FrameworkOptions.UseHttpsRedirection = false;

                app.UseFrameworkMiddlewares(null, FrameworkOptions);

                MapActions(app);
            });
    }

    [TestMethod]
    public void Disable_All_Options_Executes_Without_Error_Returns_IsOk()
    {
        DisableFrameworkOptions();
        CreateWebHostBuilder();

        var text = GetResponseText("/" + nameof(Disable_All_Options_Executes_Without_Error_Returns_IsOk_Action));

        IsOk(text);
    }

    public string Disable_All_Options_Executes_Without_Error_Returns_IsOk_Action()
    {
        return "Disable_All_Options_Executes_Without_Error_Returns_IsOk";
    }

    [TestMethod]
    public void UseMvc_Without_Controller_To_Match_Returns_IsOk()
    {
        DisableFrameworkOptions();
        
        FrameworkOptions.UseMvc = true;

        CreateWebHostBuilder();

        var text = GetResponseText("/" + nameof(UseMvc_Without_Controller_To_Match_Returns_IsOk_Action));

        IsOk(text);
    }

    public string UseMvc_Without_Controller_To_Match_Returns_IsOk_Action()
    {
        return "UseMvc_Without_Controller_To_Match_Returns_IsOk";
    }

    [TestMethod]
    public void Default_IsOk()
    {
        CreateWebHostBuilder();

        var text = GetResponseText("/" + nameof(Default_IsOk_Action));

        IsOk(text);
    }

    public string Default_IsOk_Action()
    {
        return "Default_IsOk";
    }
}