using Microsoft.AspNetCore.Mvc;

namespace SystemLibrary.Common.Framework.App;

[ApiTokenFilter(match: "ABCdef123,.-_?$@#:;()<>!")]
public class ApiTokenFilterController : BaseApiController
{
    [HttpGet]
    [Route("/api/apitokenfilter/")]
    public ActionResult ApiTokenFilter(int i) => Ok();
}
