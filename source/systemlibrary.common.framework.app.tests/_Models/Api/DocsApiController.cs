using Microsoft.AspNetCore.Mvc;

namespace SystemLibrary.Common.Framework.App;

// CLASS DEFINED ALL METHODS AND HOW THEY ARE PRINTED
public class DocsApiController : BaseApiController
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

    /// <summary>
    /// For applications that allow Ambiguity methods, supporting overloaded methods in the Routing mechanism
    /// We make sure those are printed OK too
    /// </summary>
    [HttpGet]
    public ActionResult Ambiguity() => Ok();

    [HttpGet]
    [Route("/root/ambiguity/{f}")]
    public ActionResult Ambiguity(float f) => Ok();

    [HttpGet]
    public ActionResult Ambiguity(double d) => Ok();

    [HttpGet]
    public ActionResult Ambiguity(ProductColor color1, ProductColor color2 = ProductColor.Red, ProductColor color3 = (ProductColor)4) => Ok();

    [HttpGet]
    public ActionResult Ambiguity(decimal dd) => Ok();

    [HttpGet]
    public ActionResult Ambiguity(string countryCode) => Ok();

    [HttpGet]
    public ActionResult Ambiguity(int x, int y = -1, bool b = false, bool b2 = true) => Ok();

    [HttpGet]
    public ActionResult Ambiguity(GeoLocation geoLocation) => Ok();

    [HttpGet]
    public ActionResult Ambiguity(IList<string> list) => Ok();

    [HttpGet]
    public ActionResult Ambiguity(Dictionary<string, int> dict) => Ok();

    [HttpGet]
    public ActionResult Ambiguity(IEnumerable<int> enumerable) => Ok();

    [HttpGet]
    public ActionResult Ambiguity(List<bool> boolList) => Ok();

    [HttpGet]
    public ActionResult Ambiguity(Tuple<bool, int, string, DateTime> tuple) => Ok();

    [HttpGet]
    public ActionResult Ambiguity(IDictionary<int, string> idict) => Ok();

    [HttpPost]
    public ActionResult Ambiguity([FromBody] GeoLocation geoLocation, GeoLocation geoLocationQuery) => Ok();

    [HttpPost]
    public ActionResult Ambiguity(int[] arrayX, int[] arrayY, [FromBody] GeoLocation geoLocation) => Ok();

    [HttpPost]
    public ActionResult Ambiguity(int intx, [FromBody] GeoLocation geoLocation, int inty) => Ok();

    [HttpPost]
    public ActionResult<List<GeoLocation>> Ambiguity(int intx, bool flag, [FromBody] GeoLocation geoLocation, int inty) => Ok();

    [HttpPost]
    public ActionResult Ambiguity(string x,
        [FromBody] GeoLocation geoLoc1,
        [FromBody] GeoLocation geoLoc2
    ) => Ok();
}
