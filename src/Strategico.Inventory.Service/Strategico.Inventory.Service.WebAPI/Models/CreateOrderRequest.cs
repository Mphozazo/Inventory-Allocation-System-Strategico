namespace Strategico.Inventory.Service.WebAPI.Models
{
    public class CreateOrderRequest
    {
        public List<CreateOrderLineRequest> Lines { get; set; } = new(); 
    }
}
