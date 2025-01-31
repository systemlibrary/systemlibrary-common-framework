using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using SystemLibrary.Common.Framework.App;
namespace SystemLibrary.Common.Framework.App;

public class GeoLocation
{
    public int X { get; set; }
    public int Y { get; set; }
    public Inner Inner { get; set; }
}
