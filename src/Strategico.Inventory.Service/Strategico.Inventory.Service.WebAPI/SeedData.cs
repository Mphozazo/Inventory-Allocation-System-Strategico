using Strategico.Inventory.Api.Models;
using Strategico.Inventory.Service.WebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Strategico.Inventory.Api.Data
{
    public static class SeedData
    {
        public static void Initialize(InventoryDbContext db)
        {
            // Ensure database exists
            db.Database.EnsureCreated();

            // Prevent reseeding
            if (db.Warehouses.Any() ||  db.Products.Any() || db.Stocks.Any())
                return;

            // 1. Create Warehouse
            var warehouse = new Warehouse
            {
                Id = Guid.NewGuid(),
                Name = "Main Warehouse"
            };
            db.Warehouses.Add(warehouse);
            db.SaveChanges(); // <--- Save Warehouse first

            // 2. Create Products
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
            db.Products.AddRange(laptop, phone);
            db.SaveChanges(); // <--- Save Products before Stocks

            // 3. Create Stocks (depends on Product & Warehouse)
            var stocks = new[]
            {
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
            };

            db.Stocks.AddRange(stocks);
            db.SaveChanges(); // <--- Save Stocks
        }
    }
}
