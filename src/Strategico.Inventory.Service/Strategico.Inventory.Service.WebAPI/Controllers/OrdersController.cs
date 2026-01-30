using Strategico.Inventory.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Strategico.Inventory.Api.Models;

namespace Strategico.Inventory.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly InventoryDbContext _db;

    public OrdersController(InventoryDbContext db)
    {
        _db = db;
    }

    [HttpPost]
    public async Task<IActionResult> PlaceOrder(CreateOrderRequest req)
    {
        using var tx = await _db.Database.BeginTransactionAsync();
        try
        {
            var rowsAffected = await _db.Database.ExecuteSqlRawAsync(@"
                            UPDATE ""Stocks""
                            SET ""ReservedQuantity"" = ""ReservedQuantity"" + {0}
                            WHERE ""ProductId"" = {1}
                            AND (""TotalQuantity"" - ""ReservedQuantity"") >= {0};
                            ", req.Quantity, req.ProductId);


            if (rowsAffected == 0)
            {
                await tx.RollbackAsync();
                return BadRequest("Insufficient stock in this warehouse");
            }

            var order = new Order
            {
                Id = Guid.NewGuid(),
                WarehouseId = _db.Stocks.Select(s => s.WarehouseId).First(),
                CustomerId = req.CustomerId,
                CreatedAt = DateTime.UtcNow,
                Status = "CONFIRMED"
            };

            _db.Orders.Add(order);

            _db.OrderLines.Add(new OrderLine
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                ProductId = req.ProductId,
                Quantity = req.Quantity
            });

            await _db.SaveChangesAsync();
            await tx.CommitAsync();

            return Ok(new { orderId = order.Id });
        }
        catch (Exception ex)
        {
            // return error response and rollback 
            await tx.RollbackAsync();
            return StatusCode(500, "An error occurred while placing the order: " + ex.Message);
        }
        
    }
}

public class CreateOrderRequest
{
    public string CustomerId { get; set; } = string.Empty;
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}