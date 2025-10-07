namespace backend.Models.Order
{
    
    public class CreateOrderRequest
    {
        public long TelegramUserId { get; set; }
        public List<Entities.CartItem> Items { get; set; } = new();
        public Entities.CustomerInfo CustomerInfo { get; set; } = new();
    }
    
}