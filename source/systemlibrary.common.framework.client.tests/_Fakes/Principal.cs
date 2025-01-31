using System.Security.Claims;
using System.Security.Principal;

namespace SystemLibrary.Common.Framework.App.Tests;

partial class Fakes
{
    public static IPrincipal GetPrincipal()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.Name, "unit test"),
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim(ClaimTypes.Role, "Administrator"),
            new Claim("unit-test-claim", "unit test claim"),
        }, "test"));

        return user;
    }
}
