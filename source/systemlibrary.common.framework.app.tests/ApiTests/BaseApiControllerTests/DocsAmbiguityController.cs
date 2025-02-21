using Microsoft.AspNetCore.Mvc;

namespace SystemLibrary.Common.Framework.App.ApiTests.Another.And.Deeply.NESTED;

/// <summary>
/// Applications that may allow ambiguity methods due to overwriting default MS behavior, and/or
/// supports various ambiguity routes through Route attribute
/// </summary>
public class DocsAmbiguityController : BaseApiController
{
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
