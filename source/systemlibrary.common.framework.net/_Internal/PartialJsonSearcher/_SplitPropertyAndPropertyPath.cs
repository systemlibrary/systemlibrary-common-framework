using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework;

internal static partial class PartialJsonSearcher
{
    static (string, string[]) SplitPropertyAndPropertyPath<T>(string propertySearchPath)
    {
        string property;
        string[] propertyPaths = null;

        //TODO: update to one stringbuilder/span/...
        if (propertySearchPath?.ContainsAny("/", "\\") == true)
        {
            if (propertySearchPath[0] == '~')
                propertySearchPath = propertySearchPath.Substring(1);

            propertySearchPath = propertySearchPath.TrimEnd("/", "\\");

            var paths = propertySearchPath.Trim().Split(new char[] { '/', '\\' });

            property = paths[^1];

            propertySearchPath = propertySearchPath.Substring(0, propertySearchPath.LastIndexOf(property));
            propertySearchPath = propertySearchPath.TrimEnd("/", "\\");

            propertyPaths = propertySearchPath.Split(new char[] { '/', '\\' });
        }
        else if (propertySearchPath.Is())
        {
            property = propertySearchPath;
        }
        else
        {
            var type = typeof(T);
            property = type.GetTypeName();
            if (type.IsListOrArray())
                property += "s";
        }

        return (property, propertyPaths);
    }
}
