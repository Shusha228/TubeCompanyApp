using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models.Entities
{
    /// <summary>
    /// Активная корзина пользователя
    /// </summary>
    public class CartItem
    {
        [Key, Column(Order = 0)]
        public int UserId { get; set; }
        
        [Key, Column(Order = 1)]
        public int ProductId { get; set; }
        
        public string ProductName { get; set; } = string.Empty;
        
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Quantity { get; set; }
        
        public bool IsInMeters { get; set; } = true;
        
        [Required]
        [Range(0, double.MaxValue)]
        public decimal FinalPrice { get; set; }
        
        [Required]
        [Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; }
        
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Навигационное свойство
        public virtual Nomenclature? Product { get; set; }
    }
}