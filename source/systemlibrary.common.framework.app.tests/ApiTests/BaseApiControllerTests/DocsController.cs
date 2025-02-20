using Microsoft.AspNetCore.Mvc;

namespace SystemLibrary.Common.Framework.App;

public class DocsController : BaseApiController
{
    public ActionResult GetDocumentationId(int i)
    {
        return Ok();
    }

    [HttpGet]
    public ActionResult GetDocumentations()
    {
        return Ok();
    }

    [HttpPost]
    public ActionResult PostDocumentations()
    {
        return Ok();
    }

    [HttpPost]
    public ActionResult PostDocumentationId(string id)
    {
        return Ok();
    }

    [HttpHead]
    public ActionResult HeadDocumentations()
    {
        return Ok();
    }

    [HttpPut]
    public ActionResult PutDocumentationId(string id, string optional = null)
    {
        return Ok();
    }
}
