using Microsoft.AspNetCore.Mvc;

namespace SystemLibrary.Common.Framework.App;

[OriginFilter(match: "^[ab0-4]{4,}$")]
public class OriginFilterRegexController : BaseApiController
{
    [HttpGet]
    [Route("/api/OriginFilterRegex/")]
    public ActionResult OriginFilterRegex(int i) => Ok();
}
