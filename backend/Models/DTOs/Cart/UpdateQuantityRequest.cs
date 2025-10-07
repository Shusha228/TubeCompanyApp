using System.ComponentModel;

namespace backend.Models.DTOs.Cart
{
    public class UpdateQuantityRequest
    {
        [Description("Новое количество товара")]
        public decimal NewQuantity { get; set; }
    }
}