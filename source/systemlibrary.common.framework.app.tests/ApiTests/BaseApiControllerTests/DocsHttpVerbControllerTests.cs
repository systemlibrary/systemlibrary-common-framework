using Microsoft.AspNetCore.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.App.Extensions;
using SystemLibrary.Common.Framework.Tests;

namespace SystemLibrary.Common.Framework.App;

[TestClass]
public class DocsHttpVerbControllerTests : BaseTest
{
    public DocsHttpVerbControllerTests()
    {
        WebHostBuilder = new WebHostBuilder()
            .ConfigureServices(services =>
            {
                var options = new FrameworkServiceOptions();

                options.ApplicationParts = [
                    typeof(DocsHttpVerbControllerTests).Assembly
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
    public void Is_Available_At_First_Namespace_Segment_Returns_Ok()
    {
        var text = GetResponseText("/ApiTests/DocsHttpVerb/docs");

        IsOk(text);
    }

    [TestMethod]
    public void Returns_Formatted_Docs()
    {
        var text = GetResponseText("/ApiTests/DocsHttpVerb/docs");

        var expectedLines = Assemblies.GetEmbeddedResource("DocsHttpVerbFormat.json").Split(Environment.NewLine);

        foreach (var line in expectedLines)
        {
            IsOk(text.Contains(line), "Line missing or invalid: " + line);
        }
    }
}
