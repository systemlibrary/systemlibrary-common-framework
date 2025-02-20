using System.ComponentModel;
using System.Reflection;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework.App.Extensions;

/// <summary>
/// Extension methods for IServiceCollection
/// </summary>
public static partial class IServiceCollectionExtensions
{
    /// <summary>
    /// Configures ServiceCollection in one-line, so register all of your own or other service configurations after this one
    /// <para>Registers:</para>
    /// - MVC services
    /// <para>- Razor Page services</para>
    /// - Routing services
    /// <para>- ForwardedProtocol and ForwardedIp (XForwardedFor) headers</para>
    /// - Compression for Gzip and Brotli services
    /// <para>- Authentication and authorization services</para>
    /// - Output cache services
    /// <para>- Registers the main assembly and all its controllers (if any), as in: your Web Application Project's assembly</para>
    /// Optionally, through the argument FrameworkServicesOptions: 
    /// <para>- Can register view locations</para>
    /// - Can register area view locations
    /// <para>- Can register one ViewLocationExpander</para>
    /// - and more...
    /// </summary>
    /// <example>
    /// Startup.cs/Program.cs:
    /// <code>
    /// public void ConfigureServices(IServiceCollection services)
    /// {
    ///     var options = new FrameworkServicesOptions();
    ///     
    ///     options.ViewLocations = new string[] {
    ///         "~/Views/{0}/index.cshtml"
    ///     }
    ///     
    ///     options.AreaViewLocations = new string[] {
    ///         "~/Area/{2}/{1}/{0}.cshtml"
    ///     }
    ///     
    ///     options.ViewLocationExpander = null; //or create one based on the Interface 'IViewLocationExpander'
    /// 
    ///     options.UseDataProtectionPolicy = true; // Register and enabled the data protection policy in this framework
    ///     
    ///     services.AddFrameworkServices&lt;TLogWriter&gt;(options);
    /// }
    /// </code>
    /// </example>
    public static IServiceCollection AddFrameworkServices<TLogWriter>(this IServiceCollection serviceCollection, FrameworkServiceOptions options = null) where TLogWriter : class, ILogWriter
    {
        if (serviceCollection == null) serviceCollection = new ServiceCollection();

        serviceCollection.AddCommonServices<TLogWriter>();

        options ??= new FrameworkServiceOptions();

        if (options.UseExtendedEnumModelConverter)
        {
            var enumType = typeof(Enum);
            var enumTypeConverter = TypeDescriptor.GetConverter(enumType);
            if (enumTypeConverter == null || !(enumTypeConverter is ExtendedEnumConverter))
            {
                TypeDescriptor.AddAttributes(enumType, new TypeConverterAttribute(typeof(ExtendedEnumConverter)));
            }
        }

        if (options.UseForwardedHeaders)
            serviceCollection = serviceCollection.UseForwardedHeaders();

        if (options.UseHttpsRedirection)
            serviceCollection.AddHttpsRedirection(opt => opt.HttpsPort = 443);

        if (options.UseGzipResponseCompression)
            serviceCollection = serviceCollection.UseGzipCompression();

        if (options.UseBrotliResponseCompression)
            serviceCollection = serviceCollection.UseBrotliCompression();

        if (options.UseOutputCache)
            serviceCollection.AddOutputCache(opt1 =>
            {
                opt1.SizeLimit = 2500L * 1024 * 1024;       //2.5GB
                opt1.MaximumBodySize = 8 * 1024 * 1024;     //8MB
                opt1.UseCaseSensitivePaths = false;
            });

        if (options.UseResponseCaching)
        {
            serviceCollection.AddResponseCaching(opt2 =>
            {
                opt2.SizeLimit = 2500L * 1024 * 1024;       //2.5GB
                opt2.MaximumBodySize = 8 * 1024 * 1024;     //8MB
                opt2.UseCaseSensitivePaths = false;
            });
        }

        serviceCollection.UseDataProtectionPolicy(options);

        serviceCollection.AddAuthentication()
             .AddCookie(opt =>
             {
                 opt.Cookie.SameSite = SameSiteMode.Strict;
                 opt.Cookie.HttpOnly = true;
             });

        serviceCollection.AddAuthorization();

        IMvcBuilder builder = serviceCollection.UseModelViewControllers(options);

        builder = builder.UseDefaultJsonConverters();

        builder = AddApplicationParts(builder, options);
        
        if (options.AddRazorRuntimeCompilationOnSave)
            builder = AddRazorRuntimeCompilationOnSave(builder);

        if (builder != null)
            serviceCollection = builder.Services;

        serviceCollection = serviceCollection.UseViews(options);

        if (options.UseCookiePolicy)
            serviceCollection = serviceCollection.UseCookiePolicy();

        if (options.ForwardStandardLogging)
            serviceCollection.AddLogging(bld =>
            {
                bld.AddProvider(new InternalLogProvider());
            });

        // NOTE: Can this be Scoped instead?
        serviceCollection.TryAddTransient<HtmlHelperFactory, HtmlHelperFactory>();

        if (options.AllowSynchronousIO)
        {
            serviceCollection.Configure<IISServerOptions>(options => { options.AllowSynchronousIO = true; });
            serviceCollection.Configure<KestrelServerOptions>(options => { options.AllowSynchronousIO = true; });
        }

        return serviceCollection;
    }
}