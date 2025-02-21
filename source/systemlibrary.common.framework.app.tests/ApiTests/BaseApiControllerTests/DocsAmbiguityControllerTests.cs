using Microsoft.AspNetCore.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.App.Extensions;
using SystemLibrary.Common.Framework.Tests;

namespace SystemLibrary.Common.Framework.App;

[TestClass]
public class DocsAmbiguityControllerTests : BaseTest
{
    public DocsAmbiguityControllerTests()
    {
        WebHostBuilder = new WebHostBuilder()
            .ConfigureServices(services =>
            {
                var options = new FrameworkServiceOptions();

                options.ApplicationParts = [
                    typeof(DocsAmbiguityControllerTests).Assembly
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
    public void Is_Available_At_Deeply_Nested_Namespace_Segment_Returns_Ok()
    {
        var text = GetResponseText("/ApiTests/AnOTHer/and/Deeply/nested/DocsAmbiguity/docs");

        IsOk(text);
    }

    [TestMethod]
    public void Returns_Formatted_Docs()
    {
        var text = GetResponseText("/ApiTests/AnOTHer/and/Deeply/nested/DocsAmbiguity/docs");

        var expectedLines = Assemblies.GetEmbeddedResource("DocsAmbiguityFormat.json").Split(Environment.NewLine);

        foreach (var line in expectedLines)
        {
            IsOk(text.Contains(line), "Line missing or invalid: " + line);
        }
    }
}
