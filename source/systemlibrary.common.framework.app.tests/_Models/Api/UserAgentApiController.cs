using Microsoft.AspNetCore.Mvc;

namespace SystemLibrary.Common.Framework.App;

[UserAgentFilter(match: "He.l.lo-User-Agent;(SomeOS)")]
public class UserAgentApiController : BaseApiController
{
    [HttpGet]
    [Route("/userAgent/getPin/")]
    public ActionResult GetPin() => Ok();
}
