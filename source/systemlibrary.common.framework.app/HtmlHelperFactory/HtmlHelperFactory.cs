﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace SystemLibrary.Common.Framework.App;

/// <summary>
/// HtmlHelperFactory builds a new instance of IHtmlBuilder outside of your View Context
/// </summary>
public class HtmlHelperFactory
{
    //Creds to: https://stackoverflow.com/questions/42039269/create-custom-html-helper-in-asp-net-core/51466436#51466436

    static ModelStateDictionary ModelStateDictionary = new ModelStateDictionary();
    static HtmlHelperOptions HtmlHelperOptions = new HtmlHelperOptions();
    static ControllerActionDescriptor ControllerActionDescriptor = new ControllerActionDescriptor();
    static DummyIndexView DummyIndex = new DummyIndexView();
    static ITempDataProvider TempDataProvider;

    /// <summary>
    /// Build a IHtmlHelper&lt;T&gt; where T is your ViewModel
    /// </summary>
    /// <example>
    /// Usage:
    /// <code>
    /// class ViewModel 
    /// {
    ///     public string Title { get; set; }
    /// }
    /// var htmlHelper = HtmlHelperFactory.Build&lt;ViewModel&gt;();
    /// </code>
    /// </example>
    /// <returns>IHtmlHelper of T instance, never null</returns>
    public static IHtmlHelper<T> Build<T>() where T : class
    {
        var viewContext = GetViewContext<T>();

        var htmlHelper = Services.Get<IHtmlHelper<T>>();

        ((IViewContextAware)htmlHelper).Contextualize(viewContext);

        return htmlHelper;
    }

    /// <summary>
    /// Build a default IHtmlHelper 
    /// </summary>
    /// <example>
    /// Usage:
    /// <code>
    /// var htmlHelper = HtmlHelperFactory.Build();
    /// </code>
    /// </example>
    /// <returns>IHtmlHelper instance, never null</returns>
    public static IHtmlHelper Build()
    {
        var viewContext = GetViewContext();

        var htmlHelper = Services.Get<IHtmlHelper>();

        ((IViewContextAware)htmlHelper).Contextualize(viewContext);

        return htmlHelper;
    }

    static ViewContext GetViewContext<T>()
    {
        var httpContext = HttpContextInstance.Current;

        var modelMetadataProvider = httpContext.RequestServices.GetRequiredService<IModelMetadataProvider>();

        var viewData = new ViewDataDictionary<T>(modelMetadataProvider, ModelStateDictionary);

        return GetViewContext(httpContext, viewData);
    }

    static ViewContext GetViewContext()
    {
        var httpContext = HttpContextInstance.Current;

        var modelMetadataProvider = httpContext.RequestServices.GetRequiredService<IModelMetadataProvider>();

        var viewData = new ViewDataDictionary(modelMetadataProvider, ModelStateDictionary);

        return GetViewContext(httpContext, viewData);
    }

    static ViewContext GetViewContext(HttpContext httpContext, ViewDataDictionary viewData)
    {
        TempDataProvider ??= httpContext.RequestServices.GetRequiredService<ITempDataProvider>();

        var tempData = new TempDataDictionary(httpContext, TempDataProvider);

        return new ViewContext(
            new ActionContext(httpContext, httpContext.GetRouteData(), ControllerActionDescriptor),
            DummyIndex,
            viewData,
            tempData,
            TextWriter.Null,
            HtmlHelperOptions
        );
    }

    internal class DummyIndexView : IView
    {
        public Task RenderAsync(ViewContext context)
        {
            return Task.CompletedTask;
        }

        public string Path => "Index";
    }
}