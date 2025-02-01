using System.ComponentModel;
using System.Reflection;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
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
    // NOTE: Whats the need of having this ? IServices should just do that, register all the services we want, period, no serviceprovider is here yet...
    // But we could register the ServiceCollection, a ref to the instance, so if something invokes something and currprovider is null we could "build it" based on "curr state" of that ref... we will see
    ///// <summary>
    ///// Configures ServiceCollection in one-line, registering the most common services and building the service provider and returns it
    ///// <para>Registers:</para>
    ///// - MVC services
    ///// <para>- Razor Page services</para>
    ///// - Routing services
    ///// <para>- ForwardedProtocol and ForwardedIp (XForwardedFor) headers</para>
    ///// - Compression for Gzip and Brotli services
    ///// <para>- Authentication and authorization services</para>
    ///// - Output cache services
    ///// <para>- Registers the main assembly and all its controllers (if any), as in: your Web Application Project's assembly</para>
    ///// Optionally, through the argument FrameworkServicesOptions: 
    ///// <para>- and more...</para>
    ///// </summary>
    //public static IServiceProvider UseFrameworkServices<TLogWriter>(this IServiceCollection serviceCollection, FrameworkServicesOptions options = null) where TLogWriter : class, ILogWriter
    //{
    //    return serviceCollection
    //        .AddFrameworkServices<TLogWriter>(options)
    //        .BuildServiceProvider()
    //        .UseFrameworkServiceProvider();
    //}

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
    ///     services.AddFrameworkServices(options);
    /// }
    /// </code>
    /// </example>
    public static IServiceCollection AddFrameworkServices<TLogWriter>(this IServiceCollection serviceCollection, FrameworkServicesOptions options = null) where TLogWriter : class, ILogWriter
    {
        if (serviceCollection == null) serviceCollection = new ServiceCollection();

        serviceCollection.AddCommonServices<TLogWriter>();

        options ??= new FrameworkServicesOptions();

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
                opt1.SizeLimit = 2000L * 1024 * 1048;       //2GB
                opt1.MaximumBodySize = 8 * 1024 * 1024;     //8MB
                opt1.UseCaseSensitivePaths = false;
            });

        if (options.UseResponseCaching)
        {
            serviceCollection.AddResponseCaching(opt2 =>
            {
                opt2.SizeLimit = 2000L * 1024 * 1048;       //2GB
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

        IMvcBuilder builder = null;

        if (options.UseMvc)
        {
            builder = serviceCollection.AddMvc(options =>
            {
            });
            
            if (options.UseRazorPages)
                builder = serviceCollection.UseAddRazorPages(options, builder);
        }
        else if (options.UseRazorPages)
        {
            builder = serviceCollection.UseAddRazorPages(options, builder);
        }
        else
        {
            builder = serviceCollection.UseAddControllers(options);
        }

        builder = builder.UseDefaultJsonConverters();

        if (options.AddApplicationAsPart)
        {
            var executingAssembliy = Assembly.GetExecutingAssembly();
            var entryAssembly = Assembly.GetEntryAssembly();
            var callingAssembly = Assembly.GetCallingAssembly();

            builder = AddApplicationPart(builder, executingAssembliy, entryAssembly, callingAssembly);
        }

        if (options.ApplicationParts != null)
        {
            foreach (var part in options.ApplicationParts)
                if (part != null)
                    builder = builder.AddApplicationPart(part);
        }

        if (options.AddRazorRuntimeCompilationOnChange)
            builder = AddRazorRuntimeCompilationOnChange(builder);

        if (builder != null)
            serviceCollection = builder.Services;

        serviceCollection = serviceCollection.UseViews(options);

        if (options.UseCookiePolicy)
            serviceCollection = serviceCollection.UseCookiePolicy();

        if (options.ForwardStandardLogging)
            serviceCollection.AddLogging(builder =>
            {
                builder.AddProvider(new InternalLogProvider());
            });

        // NOTE: Can this be Scoped instead?
        serviceCollection.TryAddTransient<HtmlHelperFactory, HtmlHelperFactory>();

        if (options.AllowSynchronousIO)
            serviceCollection.Configure<IISServerOptions>(options => { options.AllowSynchronousIO = true; });

        if (options.AllowSynchronousIO)
            serviceCollection.Configure<KestrelServerOptions>(options => { options.AllowSynchronousIO = true; });
        
        return serviceCollection;
    }
}