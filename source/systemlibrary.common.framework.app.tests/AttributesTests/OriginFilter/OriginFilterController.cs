using Microsoft.AspNetCore.Mvc;

namespace SystemLibrary.Common.Framework.App;

[OriginFilter(match: "ABCdef123,.-_?$@#:;()<>!")]
public class OriginFilterController : BaseApiController
{
    [HttpGet]
    [Route("/api/originfilter/")]
    public ActionResult OriginFilter(int i) => Ok();
}
