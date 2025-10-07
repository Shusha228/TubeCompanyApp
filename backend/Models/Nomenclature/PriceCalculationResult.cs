using System.ComponentModel;
using System.Text.Json.Serialization;

namespace backend.Models.Nomenclature
{
    /// <summary>
    /// Результат расчета стоимости
    /// </summary>
    public class PriceCalculationResult
    {
        [Description("ID номенклатуры")]
        [JsonPropertyName("nomenclatureId")]
        public int NomenclatureId { get; set; }

        [Description("Наименование номенклатуры")]
        [JsonPropertyName("nomenclatureName")]
        public string NomenclatureName { get; set; } = string.Empty;

        [Description("ID склада")]
        [JsonPropertyName("stockId")]
        public string StockId { get; set; }

        [Description("Количество")]
        [JsonPropertyName("quantity")]
        public decimal Quantity { get; set; }

        [Description("Единица измерения")]
        [JsonPropertyName("isInMeters")]
        public bool IsInMeters { get; set; }

        [Description("Базовая цена (без скидки)")]
        [JsonPropertyName("basePrice")]
        public decimal BasePrice { get; set; }

        [Description("Итоговая цена (со скидкой)")]
        [JsonPropertyName("finalPrice")]
        public decimal FinalPrice { get; set; }

        [Description("Процент скидки")]
        [JsonPropertyName("discountPercent")]
        public decimal DiscountPercent { get; set; }

        [Description("Цена за единицу")]
        [JsonPropertyName("unitPrice")]
        public decimal UnitPrice { get; set; }

        [Description("Цена за единицу со скидкой")]
        [JsonPropertyName("discountedUnitPrice")]
        public decimal DiscountedUnitPrice { get; set; }

        [Description("Доступный остаток на складе (в метрах)")]
        [JsonPropertyName("availableStock")]
        public decimal AvailableStock { get; set; }

        [Description("Доступный остаток на складе (в тоннах)")]
        [JsonPropertyName("availableStockTons")]
        public decimal AvailableStockTons { get; set; }

        [Description("Коэффициент перевода")]
        [JsonPropertyName("koef")]
        public decimal Koef { get; set; }

        [Description("Конвертированное количество")]
        [JsonPropertyName("convertedQuantity")]
        public decimal ConvertedQuantity { get; set; }

        [Description("Тип конвертации")]
        [JsonPropertyName("conversionType")]
        public string ConversionType { get; set; } = string.Empty;
    }
}