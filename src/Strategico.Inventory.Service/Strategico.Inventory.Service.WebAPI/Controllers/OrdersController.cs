using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Strategico.Inventory.Api.Data;
using Strategico.Inventory.Api.Models;
using Strategico.Inventory.Api.Services;
using Strategico.Inventory.Service.WebAPI.Models;

namespace Strategico.Inventory.Api.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly InventoryDbContext _db;
        private readonly IWarehouseProvider _warehouseProvider;

        public OrdersController(
            InventoryDbContext db,
            IWarehouseProvider warehouseProvider)
        {
            _db = db;
            _warehouseProvider = warehouseProvider;
        }
        [HttpPost]
        public async Task<IActionResult> PlaceOrder([FromBody] CreateOrderRequest request)
        {
            var warehouse = await _db.Warehouses.FirstOrDefaultAsync();
            if (warehouse == null)
                return BadRequest("No warehouse available");

            // Start transaction
            await using var tx = await _db.Database.BeginTransactionAsync();

            try
            {
                // Load stocks WITH tracking + locking
                var stocks = await _db.Stocks
                    .Where(s =>
                        s.WarehouseId == warehouse.Id &&
                        request.Lines.Select(l => l.ProductId).Contains(s.ProductId))
                    .ToListAsync();

                if (!stocks.Any())
                    return BadRequest("No stock found for selected products");

                // Validate availability
                foreach (var line in request.Lines)
                {
                    var stock = stocks.FirstOrDefault(s => s.ProductId == line.ProductId);

                    if (stock == null)
                        return BadRequest($"Stock not found for product {line.ProductId}");

                    if (stock.AvailableQuantity < line.Quantity)
                        return BadRequest($"Insufficient stock for product {line.ProductId}");
                }

                // Create order aggregate
                var order = new Order
                {
                    Id = Guid.NewGuid(),
                    WarehouseId = warehouse.Id,
                    CreatedAt = DateTime.UtcNow,
                    Status = "Placed",
                    Lines = new List<OrderLine>()
                };

                // Apply domain logic
                foreach (var line in request.Lines)
                {
                    var stock = stocks.First(s => s.ProductId == line.ProductId);

                    // concurrency tracked via Stock.Version (optimistic concurrency) 
                    stock.ReservedQuantity += line.Quantity;   
                    stock.Version += 1;                         

                    order.Lines.Add(new OrderLine
                    {
                        Id = Guid.NewGuid(),
                        OrderId = order.Id,
                        ProductId = line.ProductId,
                        Quantity = line.Quantity
                    });
                }

                _db.Orders.Add(order);

                await _db.SaveChangesAsync();   // atomic write
                await tx.CommitAsync();         // commit transaction

                return Ok(new
                {
                    OrderId = order.Id,
                    Status = order.Status
                });
            }
            catch (DbUpdateConcurrencyException)
            {
                await tx.RollbackAsync();
                return Conflict("Stock was modified by another transaction. Please retry.");
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return StatusCode(500, ex.Message);
            }
        }
    }


}
