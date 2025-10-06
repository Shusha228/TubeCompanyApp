using System.ComponentModel.DataAnnotations;

namespace backend.Models.Entities
{
    public class Order
    {
        [Key] public string Id { get; set; } = Guid.NewGuid().ToString();
        public long TelegramUserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Inn { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<CartItem> Items { get; set; } = new();
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Pending";
        public bool AdminNotified { get; set; } = false;
    }

    public class CartItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public bool IsInMeters { get; set; } = true;
        public decimal FinalPrice { get; set; }
        public decimal UnitPrice { get; set; }
        public string StockId { get; set; }
        public string Warehouse { get; set; } = string.Empty;
    }
    
    
    public class CustomerInfo
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Inn { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}