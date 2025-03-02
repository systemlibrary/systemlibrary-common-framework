using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;

namespace SystemLibrary.Common.Framework.App.Tests;

public class TvContentApiController : BaseApiController
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

[Route("api2/customUserApi")]
public class UserApiController : BaseApiController
{
    [HttpPost("createUser")]
    public ActionResult CreateUser(string firstName, string lastName, string email)
    {
        return null;
    }

    [HttpGet("getUser/{userId}")]
    public ActionResult GetUser(int userId)
    {
        return null;
    }

    [HttpPut("updateUser/{userId}")]
    public ActionResult UpdateUser(int userId, string? firstName, string? lastName, string? email)
    {
        return null;
    }

    [HttpDelete("deleteUser/{userId}")]
    public ActionResult DeleteUser(int userId)
    {
        return null;
    }
}

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

public class ProductOrder
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}