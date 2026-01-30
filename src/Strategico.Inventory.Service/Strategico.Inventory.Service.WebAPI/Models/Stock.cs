using Strategico.Inventory.Api.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Strategico.Inventory.Service.WebAPI.Models
{
    public class Stock
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; } = null!;

        [Required]
        public Guid WarehouseId { get; set; }

        [ForeignKey(nameof(WarehouseId))]
        public Warehouse Warehouse { get; set; } = null!;

        [Required]
        public int TotalQuantity { get; set; }

        [Required]
        public int ReservedQuantity { get; set; }

        // Optimistic concurrency token
        [ConcurrencyCheck]
        [Required]
        public int Version { get; set; }

        [NotMapped]
        public int AvailableQuantity => TotalQuantity - ReservedQuantity;
    }
}
