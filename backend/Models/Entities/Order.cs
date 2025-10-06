using System.ComponentModel.DataAnnotations;

namespace backend.Models.Entities
{
    public class Order
    {
        [Key]
        public string Id { get; set; } = string.Empty; // Изменено на string
        
        public long TelegramUserId { get; set; } // Изменено на long
        
        // Информация о клиенте
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Inn { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        
        public List<OrderCartItem> Items { get; set; } = new List<OrderCartItem>();
        
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "Pending"; // Исправлено на "Pending"
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        
        public bool AdminNotified { get; set; } = false;
    }
}