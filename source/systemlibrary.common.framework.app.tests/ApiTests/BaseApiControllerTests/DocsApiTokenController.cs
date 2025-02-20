using Microsoft.AspNetCore.Mvc;

namespace SystemLibrary.Common.Framework.App.ApiTests;

[ApiTokenFilter(match: "Docs")]
public class DocsApiTokenController : BaseApiController
{
    public ActionResult GetDocumentationId(int i)
    {
        return Ok();
    }
}
