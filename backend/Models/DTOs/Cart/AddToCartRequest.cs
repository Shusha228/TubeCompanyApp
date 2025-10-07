using System.ComponentModel;

namespace backend.Models.DTOs.Cart
{
    public class AddToCartRequest
    {
        [Description("Идентификатор склада")]
        public string StockId { get; set; } = string.Empty;

        [Description("Идентификатор товара")]
        public int ProductId { get; set; }

        [Description("Наименование товара")]
        public string ProductName { get; set; } = string.Empty;

        [Description("Количество товара")]
        public decimal Quantity { get; set; }

        [Description("Единица измерения (true - метры, false - тонны)")]
        public bool IsInMeters { get; set; }

        [Description("Цена за единицу")]
        public decimal UnitPrice { get; set; }

        [Description("Итоговая цена с учетом скидок")]
        public decimal FinalPrice { get; set; }
    }
}