using System.ComponentModel;
using System.Text.Json.Serialization;

namespace backend.Models
{
    /// <summary>
    /// Запрос на расчет стоимости
    /// </summary>
    public class PriceCalculationRequest
    {
        /// <summary>
        /// ID номенклатуры
        /// </summary>
        [Description("ID номенклатуры")]
        [JsonPropertyName("nomenclatureId")]
        public int NomenclatureId { get; set; }

        /// <summary>
        /// ID склада
        /// </summary>
        [Description("ID склада")]
        [JsonPropertyName("stockId")]
        public string StockId { get; set; }

        /// <summary>
        /// Количество
        /// </summary>
        [Description("Количество")]
        [JsonPropertyName("quantity")]
        public decimal Quantity { get; set; }

        /// <summary>
        /// true - расчет в метрах, false - в тоннах
        /// </summary>
        [Description("Единица измерения (true - метры, false - тонны)")]
        [JsonPropertyName("isInMeters")]
        public bool IsInMeters { get; set; } = true;

        /// <summary>
        /// Конвертировать в тонны (только если IsInMeters = true)
        /// </summary>
        [Description("Конвертировать в тонны")]
        [JsonPropertyName("convertToTons")]
        public bool ConvertToTons { get; set; } = false;

        /// <summary>
        /// Конвертировать в метры (только если IsInMeters = false)
        /// </summary>
        [Description("Конвертировать в метры")]
        [JsonPropertyName("convertToMeters")]
        public bool ConvertToMeters { get; set; } = false;
    }
}