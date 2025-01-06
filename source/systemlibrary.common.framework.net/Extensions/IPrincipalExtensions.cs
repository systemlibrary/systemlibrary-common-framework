using System.Security.Principal;

namespace SystemLibrary.Common.Framework.Extensions;

/// <summary>
/// Extension methods on System.Security.Principal.IPrincipal
/// </summary>
public static class IPrincipalExtensions
{
    /// <summary>
    /// Check if principal is in any role, case sensitive
    /// </summary>
    /// <remarks>
    /// Passing Enum for roles to match, will simply call .ToString() on the EnumKey
    /// - It ignores EnumValue attribute
    /// - For that, you would simply use ToValue() on your Enum as argument to the method
    /// </remarks>
    /// <example>
    /// Object array as argument:
    /// <code>
    /// enum AdminRoles { Admin, MasterAdmin };
    /// var roles = new object[] { AdminRoles.Admin, AdminRoles.MasterAdmin };
    /// var isInAnyRole = principal.IsInAnyRole(roles);
    /// </code>
    /// Strings as argument
    /// <code>
    /// var isInAnyRole = principal.IsInAnyRole("Admin", "Guest");
    /// </code>
    /// Enum directly as arg
    /// /// <code>
    /// var isInAnyRole = principal.IsInAnyRole(AdminRoles.Admin, AdminRoles.MasterAdmin);
    /// </code>
    /// </example>
    /// <returns>True of false</returns>
    public static bool IsInAnyRole(this IPrincipal principal, params object[] roles)
    {
        if (principal == null || roles == null) return false;

        for (int i = 0; i < roles.Length; i++)
            if (roles[i] != null && principal.IsInRole(roles[i] + ""))
                return true;

        return false;
    }
}
