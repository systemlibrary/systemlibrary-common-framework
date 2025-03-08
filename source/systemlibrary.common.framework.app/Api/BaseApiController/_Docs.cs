using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework.App;

partial class BaseApiController
{
    [HttpGet]
    public ActionResult Docs()
    {
        var endpoints = new Dictionary<string, string>();

        var controllerType = GetType();

        var methods = controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

        var apiPath = GetApiPath(controllerType);

        foreach (var method in methods)
        {
            if (method.Name == nameof(Docs)) continue;

            var httpMethods = GetHttpMethodFormatted(method).ToUpper();

            var endpointPath = GetEndpointPath(apiPath, method, controllerType);

            var endpointFullName = GetEndpointFullName(endpointPath, method);

            var routeParams = GetRouteParams(method);

            var endpointParams = GetEndpointParams(method, routeParams);

            var parameterTypes = GetParameterTypes(method);

            var response = GetResponseType(method);

            var key = $"[{httpMethods}] /{endpointFullName}{endpointParams}";

            var value = $"{parameterTypes} -> {response}";

            key = CreateOverloadedKey(endpoints, key);

            endpoints.Add(key, value);
        }

        var sortedEndpoints = endpoints
            .OrderBy(e => GetHttpMethodOrder(e.Key))
            .ThenBy(e => e.Key)
            .ToDictionary(e => e.Key, e => e.Value);

        return new ContentResult
        {
            Content = sortedEndpoints.Json(new JsonSerializerOptions()
            {
                WriteIndented = true,
                AllowTrailingCommas = true
            }),
            ContentType = "application/json",
            StatusCode = 200
        };
    }

    static string CreateOverloadedKey(Dictionary<string, string> endpoints, string key)
    {
        int counter = 1;
        var uniqueKey = key;

        while (endpoints.ContainsKey(uniqueKey))
        {
            counter++;

            uniqueKey = $"{key} ({counter})";
        }

        return uniqueKey;
    }

    static string ToCamelCase(string name)
    {
        if (name.Length <= 1)
            return name.ToLower();

        return char.ToLower(name[0]) + name.Substring(1);
    }

    static bool IsComplexType(Type type)
    {
        return !type.IsListOrArray() &&
            !type.IsPrimitive &&
            !type.IsEnum &&
            !type.Inherits(typeof(ITuple)) &&
            !type.Inherits(typeof(ICollection)) &&
            !type.Inherits(typeof(IEnumerable)) &&
            type != SystemType.StringType &&
            type != SystemType.DateTimeType &&
            type != SystemType.DateTimeTypeNullable &&
            type != SystemType.DateTimeOffsetType &&
            type != SystemType.DateTimeOffsetTypeNullable &&
            type != SystemType.DoubleType &&
            type != typeof(decimal) &&
            type != typeof(float)
            ;
    }

    static string GetApiPath(Type controllerType)
    {
        var routeTemplate = controllerType.GetCustomAttribute<RouteAttribute>()?.Template;

        if (routeTemplate.Is())
        {
            var bracketIndex = routeTemplate.IndexOf('{');

            if (bracketIndex > -1)
                return routeTemplate.Substring(0, bracketIndex).Trim('/');
        }

        var path = controllerType.Name;

        if (path.EndsWith("Controller"))
            path = path.Replace("Controller", "");

        var namespacePath = UseApiControllersRouting.GetNamespaceAsPath(controllerType);

        if (namespacePath == "")
            return $"{ToCamelCase(path)}";

        return $"{namespacePath}/{ToCamelCase(path)}";
    }

    static string GetHttpMethodFormatted(MethodInfo method)
    {
        var attributes = method.GetCustomAttributes<HttpMethodAttribute>();

        var list = new List<string>();
        foreach (var attribute in attributes)
        {
            if (attribute?.HttpMethods?.FirstOrDefault() != null)
            {
                list.AddRange(attribute.HttpMethods);
            }
        }
        if (list.Count > 0)
            return string.Join(", ", list);

        return "GET";
    }

    static string GetEndpointPath(string apiPath, MethodInfo method, Type controllerType)
    {
        var routeAttribute = method.GetCustomAttribute<RouteAttribute>();
        var routeTemplate = routeAttribute?.Template;

        if (routeTemplate.Is())
        {
            return routeTemplate.Trim('/');
        }

        return apiPath;
    }

    static string GetEndpointFullName(string endpointPath, MethodInfo method)
    {
        if (endpointPath.Contains("{")) return endpointPath;

        return endpointPath + "/" + ToCamelCase(method.Name);
    }

    static string GetEndpointParams(MethodInfo method, List<string> routeParams)
    {
        var parameters = method.GetParameters();

        var queryParams = parameters
            .Where(p => p?.GetCustomAttribute<FromBodyAttribute>() == null &&
                       !routeParams.Contains(p.Name, StringComparer.OrdinalIgnoreCase))
            .Select(p =>
            {
                var paramName = ToCamelCase(p.Name);

                if (IsComplexType(p.ParameterType))
                {
                    var properties = p.ParameterType.GetProperties()
                        .Where(prop => prop.CanRead)
                        .Select(prop =>
                        {
                            var ignored = prop.GetCustomAttribute<JsonIgnoreAttribute>();

                            if (ignored != null) return "";

                            if (IsComplexType(prop.PropertyType))
                            {
                                var innerProps = prop.PropertyType.GetProperties()
                                    .Where(innerprop => innerprop.CanRead)
                                    .Select(innerprop =>
                                    {
                                        var innerIgnored = innerprop.GetCustomAttribute<JsonIgnoreAttribute>();
                                        if (innerIgnored != null) return "";

                                        return ToCamelCase(innerprop.Name);
                                    });

                                return string.Join("&", innerProps).Replace("&&", "&");
                            }
                            else
                            {
                                return ToCamelCase(prop.Name);
                            }
                        });

                    return string.Join("&", properties).Replace("&&", "&");
                }
                else if (p.ParameterType.IsDictionary())
                {
                    var firstGenericType = p.ParameterType.GetTypeArgument();
                    var secondGenericType = p.ParameterType.GetTypeArgument(1);

                    return paramName;
                    // + "[" + (GetTranslatedParameterType(firstGenericType) ?? ToCamelCase(firstGenericType.Name)) + "]" +
                    // "=" + (GetTranslatedParameterType(secondGenericType) ?? ToCamelCase(secondGenericType.Name));
                }
                else if (p.ParameterType.IsEnum)
                {
                    var keyValue = paramName;

                    var defaultValue = GetDefaultValue(p);

                    var value = (defaultValue as Enum).ToValue();

                    return keyValue + "=" + value;
                }
                else
                {
                    var keyValue = paramName;

                    var defaultValue = GetDefaultValue(p);

                    if (defaultValue != null)
                    {
                        var value = defaultValue.ToString();

                        if (value != "0" && value != "False")
                        {
                            if (value == "True")
                                value = ToCamelCase(value);

                            keyValue += "=" + value.MaxLength(16);
                        }
                    }

                    return keyValue;
                }
            })
            .Where(q => q.Is())
            .ToList();

        if (queryParams.Any())
        {
            return "?" + string.Join("&", queryParams).TrimStart('&');
        }

        if (parameters.Is())
            return "?";

        return "";
    }

    static string GetTranslatedParameterType(Type paramType)
    {
        if (paramType.IsEnum) return "enum";

        if (paramType == SystemType.Int16Type) return "int";
        if (paramType == SystemType.Int16TypeNullable) return "number";
        if (paramType == SystemType.IntType) return "int";
        if (paramType == SystemType.IntTypeNullable) return "number";
        if (paramType == SystemType.Int64Type) return "number";
        if (paramType == SystemType.Int64TypeNullable) return "number";
        if (paramType == SystemType.DoubleType) return "number";
        if (paramType == SystemType.DoubleTypeNullable) return "number";

        if (paramType == typeof(float)) return "number";
        if (paramType == typeof(decimal)) return "number";

        if (paramType == SystemType.DateTimeType) return "date";
        if (paramType == SystemType.DateTimeTypeNullable) return "date";
        if (paramType == SystemType.DateTimeOffsetType) return "date";
        if (paramType == SystemType.DateTimeOffsetTypeNullable) return "date";

        if (paramType == SystemType.TimeSpanType) return "time";
        if (paramType == SystemType.TimeSpanTypeNullable) return "time";

        if (paramType == SystemType.BoolType) return "bool";
        if (paramType == SystemType.BoolTypeNullable) return "bool";

        return null;
    }

    static string GetParameterType(ParameterInfo parameter)
    {
        var paramType = parameter.ParameterType;

        var translatedTypeName = GetTranslatedParameterType(paramType);

        if (translatedTypeName != null) return translatedTypeName;

        if (paramType.IsListOrArray())
        {
            var innerType = paramType.GetTypeArgument();

            translatedTypeName = GetTranslatedParameterType(innerType);

            if (translatedTypeName != null) return translatedTypeName + "[]";
        }
        else if (paramType.IsDictionary())
        {
            var firstGenericType = paramType.GetTypeArgument();
            var secondGenericType = paramType.GetTypeArgument(1);

            var type1 = GetTranslatedParameterType(firstGenericType);
            var type2 = GetTranslatedParameterType(secondGenericType);

            return "<" + (type1 ?? ToCamelCase(firstGenericType.Name)) + "," + (type2 ?? ToCamelCase(secondGenericType.Name)) + ">";

            //return parameter.Name + "[" + (type1 ?? ToCamelCase(firstGenericType.Name)) + "]:" + (type2 ?? ToCamelCase(secondGenericType.Name));
        }
        else if (IsComplexType(paramType))
        {
            var fromBodyAttribute = parameter.GetCustomAttribute<FromBodyAttribute>();

            if (fromBodyAttribute != null)
            {
                var n = ToCamelCase(parameter.Name) + ":";

                var properties = paramType.GetProperties()
                    .Where(prop => prop.CanRead)
                    .Select(prop =>
                    {
                        var ignored = prop.GetCustomAttribute<JsonIgnoreAttribute>();

                        if (ignored != null) return "";

                        if (IsComplexType(prop.PropertyType))
                        {
                            var innerProps = prop.PropertyType.GetProperties()
                                .Where(innerprop => innerprop.CanRead)
                                .Select(innerprop =>
                                {
                                    var innerIgnored = innerprop.GetCustomAttribute<JsonIgnoreAttribute>();
                                    if (innerIgnored != null) return "";

                                    return ToCamelCase(innerprop.Name);
                                });

                            return "{" + string.Join(", ", innerProps) + "}";
                        }
                        else
                        {
                            return ToCamelCase(prop.Name);
                        }
                    });
                return n + " {" + string.Join(", ", properties) + "}";
            }
        }

        var name = paramType.GetTypeName();

        return ToCamelCase(name);
    }

    static string GetResponseType(MethodInfo method)
    {
        var returnType = method.ReturnType;

        if (returnType.Inherits(typeof(ActionResult)) ||
            returnType.Inherits(typeof(ActionResult<>)))
        {
            var firstGenericType = returnType.GetTypeArgument();

            if (firstGenericType == null)
            {
                return ToCamelCase(returnType.Name);
            }

            if (firstGenericType.IsListOrArray())
            {
                var nestedFirstGenericType = firstGenericType.GetTypeArgument();

                return ToCamelCase(nestedFirstGenericType.Name) + "[]";
            }

            return ToCamelCase(firstGenericType.Name);
        }

        var genericType = returnType.GetTypeArgument();

        if (genericType == null)
        {
            var n = ToCamelCase(returnType.Name);
            return n == "actionResult" ? "response" : n;
        }

        if (genericType.IsListOrArray())
        {
            var nestedFirstGenericType = genericType.GetTypeArgument();

            return ToCamelCase(nestedFirstGenericType.Name + "[]");
        }

        var n2 = ToCamelCase(returnType.Name);
        return n2 == "actionResult" ? "response" : n2;
    }

    static string GetParameterTypes(MethodInfo method)
    {
        var parameters = method.GetParameters();

        if (parameters.Length == 0) return "no args";

        var queryParams = parameters.Where(p => p?.GetCustomAttribute<FromBodyAttribute>() == null);

        var payloadParam = parameters.Where(p => p?.GetCustomAttribute<FromBodyAttribute>() != null);

        var sortedAndFormattedParams = queryParams.Concat(payloadParam).Select(p => GetParameterType(p));

        return string.Join(", ", sortedAndFormattedParams);
    }

    static int GetHttpMethodOrder(string route)
    {
        if (route.StartsWith("[GET]")) return 1;
        if (route.StartsWith("[GET, POST]")) return 2;
        if (route.StartsWith("[GET, DELETE]")) return 3;
        if (route.StartsWith("[GET, POST, PUT]")) return 4;
        if (route.StartsWith("[GET, POST, PUT, DELETE]")) return 5;
        if (route.StartsWith("[GET, ")) return 6;
        if (route.StartsWith("[POST]")) return 20;
        if (route.StartsWith("[POST, PUT]")) return 21;
        if (route.StartsWith("[POST, PUT, DELETE]")) return 22;
        if (route.StartsWith("[POST, ]")) return 23;
        if (route.StartsWith("[PUT]")) return 30;
        if (route.StartsWith("[DELETE]")) return 40;
        if (route.StartsWith("[HEAD]")) return 100;
        return 999; // Default
    }

    static List<string> GetRouteParams(MethodInfo method)
    {
        var routeTemplate = method.GetCustomAttributes<RouteAttribute>(false).FirstOrDefault()?.Template;

        if (routeTemplate.Is())
        {
            return Regex.Matches(routeTemplate, @"\{(\w+)\}").Cast<Match>()
                         .Select(m => m.Groups[1].Value).ToList();
        }
        return new List<string>();
    }

    static object GetDefaultValue(ParameterInfo param)
    {
        return param.HasDefaultValue ? param.DefaultValue : null;
    }
}
