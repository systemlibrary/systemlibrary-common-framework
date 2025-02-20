using System.Reflection;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

using SystemLibrary.Common.Framework.App;

namespace SystemLibrary.Common.Framework.Extensions;

public class UseApiControllersRouting : IControllerModelConvention
{
    static List<string> RegisterTypeInApiOnce = new List<string>();

    public void Apply(ControllerModel controller)
    {
        var controllerType = controller.ControllerType;

        if (!controllerType.Inherits(typeof(BaseApiController))) return;

        var routeTemplate = controllerType.GetCustomAttribute<RouteAttribute>()?.Template;

        if (routeTemplate.Is()) return;

        var apiPrefix = GetNamespaceAsPath(controllerType.AsType());

        var apiPath = apiPrefix.Is() ?
                $"{apiPrefix}/[controller]/[action]" :
                $"[controller]/[action]";

        controller.Selectors.Add(new SelectorModel
        {
            AttributeRouteModel = new AttributeRouteModel(new RouteAttribute(apiPath))
        });
    }

    internal static string GetNamespaceAsPath(Type controllerType)
    {
        if (controllerType == null) return "";

        var fullNamespace = controllerType.Namespace;
        var rootNamespace = controllerType.Assembly?.GetName()?.Name;

        if (fullNamespace == rootNamespace) return "";

        var fullSegments = fullNamespace.Split('.');
        var rootSegments = rootNamespace.Split('.');

        int index = 0;
        while (index < fullSegments.Length && index < rootSegments.Length && fullSegments[index].Equals(rootSegments[index], StringComparison.OrdinalIgnoreCase))
            index++;

        var remainingSegments = fullSegments.Skip(index);

        if (remainingSegments.IsNot()) return "";

        var camelCaseSegments = remainingSegments.Select(segment => ToCamelCase(segment)).ToArray();

        return string.Join("/", camelCaseSegments);
    }

    static string ToCamelCase(string name)
    {
        if (name.Length <= 1)
            return name.ToLower();

        return char.ToLower(name[0]) + name.Substring(1);
    }
}
