using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SystemLibrary.Common.Framework.App;

//[OriginFilter(match: "^[ab0-4]{4,}$")]
[UserAgentFilter("Edg|Chrome")]
[ApiTokenFilter("helloworld")]
[Route("api/googleMaps")]
public partial class GoogleMapsController : BaseApiController
{
    [HttpGet]
    public ActionResult GetPin() => Ok();

    [HttpGet]
    [Route("/root/googleMaps/getPinning/{f}")]
    public ActionResult GetPin(float f) => Ok();

    [HttpGet]
    public ActionResult GetPin(double d) => Ok();

    [HttpGet]
    public ActionResult GetPin(LogLevel color1, LogLevel color2 = LogLevel.Warning, LogLevel color3 = (LogLevel)4) => Ok();

    [HttpGet]
    public ActionResult GetPin(decimal dd) => Ok();

    [HttpGet]
    public ActionResult GetPin(string countryCode = "Hello World") => Ok();

    [HttpGet]
    public ActionResult GetPin(int x, int y = -1, bool b = false, bool b2 = true) => Ok();

    [HttpGet]
    public ActionResult GetPin(GeoLocation geoLocation = null) => Ok();

    [HttpGet]
    public ActionResult GetPin(IList<string> list) => Ok();

    [HttpGet]
    public ActionResult GetPin(Dictionary<string, int> dict = null) => Ok();

    [HttpGet]
    public ActionResult GetPin(IEnumerable<int> enumerable = null) => Ok();

    [HttpGet]
    public ActionResult GetPin(List<bool> boolList = null) => Ok();

    [HttpGet]
    public ActionResult GetPin(Tuple<bool, int, string, DateTime> tuple) => Ok();

    [HttpGet]
    public ActionResult GetPin(IDictionary<int, string> idict) => Ok();

    [HttpPost]
    public ActionResult GetPin([FromBody] GeoLocation geoLocation, GeoLocation geoLocationQuery) => Ok();

    [HttpPost]
    public ActionResult GetPin(int[] arrayX, int[] arrayY, [FromBody] GeoLocation geoLocation) => Ok();

    [HttpPost]
    public ActionResult GetPin(int intx, [FromBody] GeoLocation geoLocation, int inty) => Ok();

    [HttpPost]
    public ActionResult<List<GeoLocation>> GetPin(int intx, bool flag, [FromBody] GeoLocation geoLocation, int inty) => Ok();

    [HttpPost]
    public ActionResult GetPin(string x,
        [FromBody] GeoLocation geoLoc1,
        [FromBody] GeoLocation geoLoc2
    ) => Ok();
}
