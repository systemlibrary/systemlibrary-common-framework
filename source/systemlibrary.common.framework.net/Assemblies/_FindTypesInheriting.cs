namespace SystemLibrary.Common.Framework;

partial class Assemblies
{
    static IEnumerable<Type> _Types = null;
    static IEnumerable<Type> Types
    {
        get
        {
            if (_Types == null)
            {
                _Types = WhiteListedAssemblies.SelectMany(asm => asm.GetTypes()).ToArray();
            }
            return _Types;
        }
    }

    static IEnumerable<Type> FindTypesInheriting(Type classType, Type classWithAttribute = null)
    {
        return Types
            .Where(type => classType.IsAssignableFrom(type) &&
                    (classWithAttribute == null || type.IsDefined(classWithAttribute, false))
        );
    }
}