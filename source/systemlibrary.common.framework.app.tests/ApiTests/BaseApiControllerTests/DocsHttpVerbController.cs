using Microsoft.AspNetCore.Mvc;

namespace SystemLibrary.Common.Framework.App.ApiTests;

public class DocsHttpVerbController : BaseApiController
{
    [HttpGet("getBy/{id}")]
    public ActionResult GetById(int id)
    {
        return null;
    }

    [HttpGet("getBy/{id}/{name}")]
    public ActionResult GetByIdAndName(int id, string name)
    {
        return null;
    }

    [HttpGet("getBy/{id}/{name}")]
    public ActionResult GetByIdNameAndCountries(int id, string name, [FromQuery] string[] countries)
    {
        return null;
    }

    [HttpPost("getByProductId/{id}")]
    public ActionResult GetByProductId(int id, [FromQuery] string firstName, [FromQuery] string? lastName)
    {
        return null;
    }

    [HttpGet]
    [HttpPost]
    public ActionResult GetAll()
    {
        return null;
    }

    public ActionResult CreateProduct(string name, decimal price)
    {
        return null;
    }

    [HttpGet("getByCategory/{categoryId}")]
    public ActionResult GetByCategory(int categoryId, [FromQuery] int pageNumber, [FromQuery] int? pageSize)
    {
        return null;
    }
}
