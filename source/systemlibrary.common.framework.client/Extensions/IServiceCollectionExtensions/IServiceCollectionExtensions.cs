using System;
using System.ComponentModel;
using System.Reflection;

using Microsoft.AspNetCore.Builder;
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
    ///     services.AddFrameworkServices(options);
    /// }
    /// </code>
    /// </example>
    public static IServiceCollection AddFrameworkServices(this IServiceCollection services, FrameworkServicesOptions options = null)
    {
        services.UseAccessorServices();

        options ??= new FrameworkServicesOptions();

        if (options.UseCustomTypeConverters)
        {
            var enumType = typeof(Enum);
            var enumTypeConverter = TypeDescriptor.GetConverter(enumType);
            if (enumTypeConverter == null || !(enumTypeConverter is GlobalEnumConverter))
            {
                TypeDescriptor.AddAttributes(enumType, new TypeConverterAttribute(typeof(GlobalEnumConverter)));
            }
        }

        if (options.UseForwardedHeaders)
            services = services.UseForwardedHeaders();

        if (options.UseHttpsRedirection)
            services.AddHttpsRedirection(opt => opt.HttpsPort = 443);

        if (options.UseGzipResponseCompression)
            services = services.UseGzipCompression();

        if (options.UseBrotliResponseCompression)
            services = services.UseBrotliCompression();

        if (options.UseOutputCache)
            services.AddOutputCache(opt1 =>
            {
                opt1.SizeLimit = 2000L * 1024 * 1048;    //2GB
                opt1.MaximumBodySize = 8 * 1024 * 1024; //8MB
                opt1.UseCaseSensitivePaths = false;
            });

        if (options.UseResponseCaching)
        {
            services.AddResponseCaching(opt2 =>
            {
                opt2.SizeLimit = 2000L * 1024 * 1048;    //2GB
                opt2.MaximumBodySize = 8 * 1024 * 1024;  //8MB
                opt2.UseCaseSensitivePaths = false;
            });
        }

        services.UseAutomaticDataProtectionPolicy(options);

        IMvcBuilder builder = null;

        if (options.UseMvc)
        {
            builder = services.AddMvc();

            if (options.UseRazorPages)
                builder = services.UseAddRazorPages(options, builder);

            builder = builder.UseDefaultJsonConverters();
        }
        else if (options.UseRazorPages)
        {
            builder = services.UseAddRazorPages(options, builder);
            builder = builder.UseDefaultJsonConverters();
        }
        else
        {
            builder = services.UseAddControllers(options);
            builder = builder.UseDefaultJsonConverters();
        }

        if (options.AddApplicationAsPart)
        {
            var executingAssembliy = Assembly.GetCallingAssembly();
            var entryAssembly = Assembly.GetEntryAssembly();
            builder = AddApplicationPart(builder, options, executingAssembliy, entryAssembly);
        }

        if (options.ApplicationParts != null)
        {
            foreach (var part in options.ApplicationParts)
                if (part != null)
                    builder = AddApplicationPart(builder, options, part, null);
        }

        if (options.AddRazorRuntimeCompilationOnChange)
            AddRazorRuntimeCompilationOnChange(builder);

        services = services.UseViews(options);

        if (options.UseCookiePolicy)
            services = services.UseCookiePolicy();

        if (options.ForwardStandardLogging)
            services.AddLogging(builder =>
            {
                builder.AddProvider(new InternalLogProvider());
            });


        // NOTE: Can this be Scoped instead?
        services.TryAddTransient<HtmlHelperFactory, HtmlHelperFactory>();

        if (options.IISAllowSynchronousIO)
            services.Configure<IISServerOptions>(options => { options.AllowSynchronousIO = true; });

        return services;
    }

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
    /// pulic class LogWriter : ILogWriter 
    /// {
    ///     // Implement interface methods
    /// }
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
    ///     services.AddFrameworkServices&lt;LogWriter&gt;(options);
    /// }
    /// </code>
    /// </example>
    public static IServiceCollection AddFrameworkServices<TLogWriter>(this IServiceCollection services, FrameworkServicesOptions options) where TLogWriter : class, ILogWriter
    {
        services = AddFrameworkServices(services, options);

        // NOTE: Was transient, now scoped, but probably can be a singleton as it gets all input
        services = services.AddScoped<ILogWriter, TLogWriter>();

        return services;
    }
}