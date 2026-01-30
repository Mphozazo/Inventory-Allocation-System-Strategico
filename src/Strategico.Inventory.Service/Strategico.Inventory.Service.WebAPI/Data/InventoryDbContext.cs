using Microsoft.EntityFrameworkCore;
using Strategico.Inventory.Api.Models;
using Strategico.Inventory.Service.WebAPI.Models;

namespace Strategico.Inventory.Api.Data
{
    public class InventoryDbContext : DbContext
    {
        public InventoryDbContext(DbContextOptions<InventoryDbContext> options)
            : base(options) { }

        public DbSet<Warehouse> Warehouses => Set<Warehouse>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Stock> Stocks => Set<Stock>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderLine> OrderLines => Set<OrderLine>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Unique stock per product per warehouse
            modelBuilder.Entity<Stock>()
                .HasIndex(x => new { x.ProductId, x.WarehouseId })
                .IsUnique();

            // Optimistic concurrency
            modelBuilder.Entity<Stock>()
                .Property(x => x.Version)
                .IsConcurrencyToken();

            // Default value for Version
            modelBuilder.Entity<Stock>()
                .Property(x => x.Version)
                .HasDefaultValue(1);

            // Relationships
            modelBuilder.Entity<Stock>()
                .HasOne<Warehouse>()
                .WithMany()
                .HasForeignKey(x => x.WarehouseId);

            modelBuilder.Entity<Stock>()
                .HasOne<Product>()
                .WithMany()
                .HasForeignKey(x => x.ProductId);

            //Deletion cascade Order -> OrderLines
            modelBuilder.Entity<Order>()
                .HasMany(o => o.Lines)
                .WithOne(l => l.Order)
                .HasForeignKey(l => l.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
