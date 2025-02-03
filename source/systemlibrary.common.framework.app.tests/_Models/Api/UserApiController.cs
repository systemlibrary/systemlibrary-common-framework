using Microsoft.AspNetCore.Mvc;
namespace SystemLibrary.Common.Framework.App;

[Route("api2/customUserApi")]
public class UserApiController : BaseApiController
{
    [HttpPost("createUser")]
    public ActionResult CreateUser(string firstName, string lastName, string email)
    {
        return null;
    }

    [HttpGet("getUser/{userId}")]
    public ActionResult GetUser(int userId)
    {
        return null;
    }

    [HttpPut("updateUser/{userId}")]
    public ActionResult UpdateUser(int userId, string? firstName, string? lastName, string? email)
    {
        return null;
    }

    [HttpDelete("deleteUser/{userId}")]
    public ActionResult DeleteUser(int userId)
    {
        return null;
    }
}
