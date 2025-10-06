using System.ComponentModel.DataAnnotations;

namespace backend.Models.Entities
{
    public class CartItem
    {
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public bool IsInMeters { get; set; } = true;
        public decimal FinalPrice { get; set; }
        public decimal UnitPrice { get; set; }
    }
}