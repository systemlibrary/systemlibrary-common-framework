using Microsoft.AspNetCore.Mvc;

namespace SystemLibrary.Common.Framework.App;

[Route("api/cartApi")]
public class CartApiController : BaseApiController
{
    [HttpPut("addToCart")]
    public ActionResult AddToCart([FromQuery] int productId, [FromQuery] int quantity)
    {
        return null;
    }

    [HttpGet("checkAvailability/{productId}")]
    public ActionResult CheckAvailability(int productId)
    {
        return null;
    }

    [HttpGet]
    public ActionResult UpdateOrder(int userId, ProductOrder product)
    {
        return null;
    }

    [HttpPost("placeOrder")]
    public ActionResult PlaceOrder(int userId, [FromBody] List<ProductOrder> products)
    {
        return null;
    }

    [HttpGet("orderStatus/{orderId}")]
    public ActionResult OrderStatus(int orderId)
    {
        return null;
    }
}
