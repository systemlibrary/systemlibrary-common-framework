using Microsoft.AspNetCore.Mvc;

namespace SystemLibrary.Common.Framework.App;

[UserAgentFilter(match: "ABCdef123,.-_?$@#:;()<>!")]
public class UserAgentFilterController : BaseApiController
{
    [HttpGet]
    [Route("/api/UserAgentFilter/")]
    public ActionResult UserAgentFilter(int i) => Ok();
}
