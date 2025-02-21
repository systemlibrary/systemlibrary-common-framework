using Microsoft.AspNetCore.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.App.Extensions;
using SystemLibrary.Common.Framework.Tests;

namespace SystemLibrary.Common.Framework.App;

[TestClass]
public class DocsControllerTests : BaseTest
{
    public DocsControllerTests()
    {
        WebHostBuilder = new WebHostBuilder()
            .ConfigureServices(services =>
            {
                var options = new FrameworkServiceOptions();

                options.ApplicationParts = [
                    typeof(DocsControllerTests).Assembly
                ];

                services = services.AddFrameworkServices(options);
            })
            .Configure(app =>
            {
                var options = new FrameworkAppOptions();

                options.UseHttpsRedirection = false;

                app.UseFrameworkMiddlewares(null, options);
            });
    }

    [TestMethod]
    public void Is_Available_At_Root_Due_To_Namespace_Is_Root_Returns_Ok()
    {
        var text = GetResponseText("/docs/docs");

        IsOk(text);
    }

    [TestMethod]
    public void Returns_Formatted_Docs()
    {
        var text = GetResponseText("/docs/docs");

        var expectedLines = Assemblies.GetEmbeddedResource("DocsControllerFormat.json").Split(Environment.NewLine);

        foreach(var line in expectedLines)
        {
            IsOk(text.Contains(line), "Line missing or invalid: " + line);
        }
    }
}
