namespace SystemLibrary.Common.Framework;

partial class Assemblies
{
    static readonly IEnumerable<Type> Types;

    static IEnumerable<Type> FindTypesInheriting(Type classType, Type classWithAttribute = null)
    {
        return Types
            .Where(type =>
                classType.IsAssignableFrom(type) &&
                type != classType &&
                (classWithAttribute == null || type.IsDefined(classWithAttribute, false))
        );
    }
}