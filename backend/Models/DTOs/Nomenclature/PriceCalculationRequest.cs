using System.ComponentModel;
using System.Text.Json.Serialization;

namespace backend.Models.Nomenclature
{

    /// Запрос на расчет стоимости
    public class PriceCalculationRequest
    {
        
        [Description("ID номенклатуры")]
        [JsonPropertyName("nomenclatureId")]
        public int NomenclatureId { get; set; }

        [Description("ID склада")]
        [JsonPropertyName("stockId")]
        public string StockId { get; set; }

        [Description("Количество")]
        [JsonPropertyName("quantity")]
        public decimal Quantity { get; set; }

        [Description("Единица измерения (true - метры, false - тонны)")]
        [JsonPropertyName("isInMeters")]
        public bool IsInMeters { get; set; } = true;

        [Description("Конвертировать в тонны")]
        [JsonPropertyName("convertToTons")]
        public bool ConvertToTons { get; set; } = false;

        [Description("Конвертировать в метры")]
        [JsonPropertyName("convertToMeters")]
        public bool ConvertToMeters { get; set; } = false;
    }
}