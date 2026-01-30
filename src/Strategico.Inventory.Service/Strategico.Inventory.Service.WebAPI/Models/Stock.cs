namespace Strategico.Inventory.Api.Models;

public class Stock
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public Guid WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = null!;
    public int TotalQuantity { get; set; }
    public int ReservedQuantity { get; set; }
    public byte[] Version { get; set; } = null!;
}