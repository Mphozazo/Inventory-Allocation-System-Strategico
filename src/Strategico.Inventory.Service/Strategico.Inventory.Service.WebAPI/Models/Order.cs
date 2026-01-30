namespace Strategico.Inventory.Api.Models;

public class Order
{
    public Guid Id { get; set; }
    public Guid WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = null!;
    public string CustomerId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = "Placed";
    public ICollection<OrderLine> Lines { get; set; } = new List<OrderLine>();
}