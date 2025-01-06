using Asm = System.Reflection.Assembly;

namespace SystemLibrary.Common.Framework;

/// <summary>
/// Static functions running on loaded Assemblies in your application
/// </summary>
/// <remarks>
/// Built on top of System.Reflection.Assembly
/// </remarks>
public static partial class Assemblies
{
    static string[] BlacklistedAssemblyNames =>
    [
        "Microsoft",
        "Docfx",
        "Windows",
        "MSBuild",
        "System.",
        "Castle.",
        "Owin",
        "StructureMap",
        "EntityFramework",
        "EPiServer",
        "Umbraco",
        "Newtonsoft",
        "Swashbuckle",
        "RestSharp",
        "GraphQL",
        "log4net",
        "MSTest",
        "VSTest",
        "Serilog",
        "nlog",
        "ElasticSearch",
        "Elasticsearch",
        "Remotion",
        "YamlDotNet",
        "Antlr",
        "ClearScript",
        "nunit",
        "AWS",
        "SharpZipLib",
        "HtmlAgilityPack",
        "Azure.",
        "JavaScriptEngineSwitcher",
        "NuGet",
        "Salesforce",
        "React",
        "moq",
        "Moq",
        "automapper",
        "AutoMapper",
        "Autofac",
        "Dapper",
        "SystemLibrary.Common.Framework.Net",
        "SystemLibrary.Common.Framework.App",
        "testhost",
        "netstandard",
        "Anonymously Hosted",
        "DynamicContentModelsAssembly",
        "nunit.",
        "xunit.",
        "Polly.",
        "runtime.win",
        "FluentValidation.",
        "FluentAssertions.",
        "StackExchange.",
        "AutoFixture.",
        "Modernizr.",
        "DocumentFormat.OpenXml",
        "NLog.",
        "IdentityModel.",
        "coverlet.",
        "MediatR.",
        "StyleCop.",
        "Hangfire.",
        "Pipelines.",
        "NUnit3TestAdapter.",
        "Npgsql.",
        "Humanizer.Core",
        "NSubstitute",
        "NJsonSchema",
        "bootstrap",
        "SendGrid",
        "Portable.BouncyCastle.",
        "RabbitMQ.",
        "SQLitePCLRaw.",
        "CsvHelper.",
        "Elasticsearch.",
        "MongoDB.",
        "WebGrease.",
        "Google.",
        "jQuery.",
        "SharpCompress.",
        "JetBrains.",
        "NodaTime.",
        "Selenium.",
        "CommandLineParser.",
        "SendGrid.",
        "WebActivatorEx.",
        "MessagePack",
        "MailKit",
        "protobuf-net.",
        "Unity.",
        "MySql.Data.",
        "Xamarin.",
        "CommonServiceLocator.",
        "NuGet.Packaging.",
        "IdentityServer4.",
        "FluentEmail",
        "Grpc."

    ];

    static IEnumerable<Asm> WhiteListedAssemblies;

    /// <summary>
    /// Find all types inheriting/implements T from all white listed loaded assemblies
    /// </summary>
    /// <remarks>
    /// Skips searching in known assemblies, names starting with Microsoft, System, EntityFramework, AWS, Serilog, MSTest, nunit, Newtonsoft, Xamarin, Dapper, Autofac, Automapper, Salesforce and more...
    /// </remarks>
    /// <example>
    /// <code class="language-csharp hljs">
    /// public class Car : IVehicle {
    /// }
    /// var vehicles = Assemblies.FindAllTypesInheriting&lt;IVehicle&gt;
    /// // returns 'Car' and all other types that inherits/implements IVehicle
    /// </code>
    /// </example>
    /// <returns>IEnumerable of System.Type</returns>
    public static IEnumerable<Type> FindAllTypesInheriting<TClassType>() where TClassType : class
    {
        return FindTypesInheriting(typeof(TClassType));
    }

    /// <summary>
    /// Find all types inheriting/implements T with a attribute from all white listed loaded assemblies
    /// </summary>
    /// <remarks>
    /// Skips searching in known assemblies, names starting with Microsoft, System, EntityFramework, AWS, Serilog, MSTest, nunit, Newtonsoft, Xamarin, Dapper, Autofac, Automapper, Salesforce and more...
    /// </remarks>
    /// <example>
    /// <code class="language-csharp hljs">
    /// public class NameAttribute : Attribute { 
    /// }
    /// 
    /// [NameAttribute]
    /// public class Car : IVehicle {
    /// }
    /// 
    /// var vehicles = Assemblies.FindAllTypesInheriting&lt;IVehicle,NameAttribute&gt;
    /// // returns 'Car' and all other types that inherits/implements IVehicle which also contains the attribute
    /// </code>
    /// </example>
    /// <typeparam name="TClassType">Class</typeparam>
    /// <typeparam name="TAttributeType">Attribute</typeparam>
    /// <returns>IEnumerable of System.Type</returns>
    public static IEnumerable<Type> FindAllTypesInheritingWithAttribute<TClassType, TAttributeType>()
        where TClassType : class
        where TAttributeType : Attribute
    {
        return FindTypesInheriting(typeof(TClassType), typeof(TAttributeType));
    }

    /// <summary>
    /// Read the content of an embedded resource as a string
    /// </summary>
    /// <remarks>
    /// Searches only in one assembly, defaulting to 'CallingAssembly'
    /// </remarks>
    /// <example>
    /// <code class="language-csharp hljs">
    /// var text = Assemblies.GetEmbeddedResource("Folder/SubFolder/SubFolder2/json.text");
    /// Assert.IsTrue(text.Contains("hello world"));
    /// // assume a file in Solution Explorer exists at "~/Folder/SubFolder/SubFolder2/json.txt"
    /// // assume "json.txt" has build action 'Embedded Resource'
    /// // assume "json.txt" contains 'hello world' this is now true
    /// </code>
    /// </example>
    public static string GetEmbeddedResource(string relativeName, Asm assembly = null)
    {
        return ReadEmbeddedResourceAsString(relativeName, assembly ?? Asm.GetCallingAssembly());
    }

    /// <summary>
    /// Read the content of an embedded resource as a byte[]
    /// </summary>
    /// <example>
    /// <code class="language-csharp hljs">
    /// var bytes = Assemblies.GetEmbeddedResourceAsBytes("Folder/SubFolder/SubFolder2/image.png");
    /// // bytes contains now the whole image.jpg as an byte array, assuming image.jpg was marked with the build action 'Embedded Resource'
    /// </code>
    /// </example>
    public static byte[] GetEmbeddedResourceAsBytes(string relativeName, Asm assembly = null)
    {
        return ReadEmbeddedResourceAsBytes(relativeName, assembly ?? Asm.GetCallingAssembly());
    }
}