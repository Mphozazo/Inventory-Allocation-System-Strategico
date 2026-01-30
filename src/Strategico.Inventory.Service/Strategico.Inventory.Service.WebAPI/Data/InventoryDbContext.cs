using Strategico.Inventory.Api.Models;
using Strategico.Inventory.Api.Services;
using Microsoft.EntityFrameworkCore;
using Strategico.Inventory.Service.WebAPI.Models;

namespace Strategico.Inventory.Api.Data;

public class InventoryDbContext : DbContext
{
    private readonly IWarehouseProvider _warehouseProvider;

    public InventoryDbContext(DbContextOptions<InventoryDbContext> options, IWarehouseProvider warehouseProvider)
        : base(options)
    {
        _warehouseProvider = warehouseProvider;
    }

    public DbSet<Warehouse> Warehouses => Set<Warehouse>();
    public DbSet<Stock> Stocks => Set<Stock>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderLine> OrderLines => Set<OrderLine>();
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Stock>()
            .HasQueryFilter(s => s.WarehouseId == _warehouseProvider.WarehouseId);

        modelBuilder.Entity<Stock>()
            .HasIndex(s => new { s.ProductId, s.WarehouseId })
            .IsUnique();

        modelBuilder.Entity<Stock>()
            .Property(s => s.Version)
            .IsRowVersion();

        base.OnModelCreating(modelBuilder);
    }
}