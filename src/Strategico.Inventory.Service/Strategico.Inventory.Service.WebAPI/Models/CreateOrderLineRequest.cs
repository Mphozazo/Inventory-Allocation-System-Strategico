namespace Strategico.Inventory.Service.WebAPI.Models
{
    public class CreateOrderLineRequest
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}