using Strategico.Inventory.Api.Models;
using Strategico.Inventory.Service.WebAPI.Models;


namespace Strategico.Inventory.Api.Data;

public static class SeedData
{
    public static void Initialize(InventoryDbContext db)
    {
        // Ensure database exists
        db.Database.EnsureCreated();

        // Prevent reseeding
        if (db.Warehouses.Any())
            return;

        // Warehouse
        var warehouse = new Warehouse
        {
            Id = Guid.NewGuid(),
            Name = "Main Warehouse"
        };

        // Products
        var laptop = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Laptop"
        };

        var phone = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Phone"
        };

        db.Warehouses.Add(warehouse);
        db.Products.AddRange(laptop, phone);

        // Stock
        db.Stocks.AddRange(
            new Stock
            {
                Id = Guid.NewGuid(),
                ProductId = laptop.Id,
                WarehouseId = warehouse.Id,
                TotalQuantity = 100,
                ReservedQuantity = 0,
                Version = 1
            },
            new Stock
            {
                Id = Guid.NewGuid(),
                ProductId = phone.Id,
                WarehouseId = warehouse.Id,
                TotalQuantity = 200,
                ReservedQuantity = 0,
                Version = 1
            }
        );

        db.SaveChanges();
    }
}
