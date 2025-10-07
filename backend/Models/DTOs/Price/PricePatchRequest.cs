using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace backend.Models.DTOs.Price
{
    public class PricePatchRequest
    {
        [Description("НДС в процентах")]
        [JsonPropertyName("nds")]
        [Range(0, 100)]
        public decimal? NDS { get; set; }
            
        [Description("Базовая цена за тонну")]
        [JsonPropertyName("priceT")]
        [Range(0, double.MaxValue)]
        public decimal? PriceT { get; set; }
            
        [Description("Пороговое количество для цены T1 (в тоннах)")]
        [JsonPropertyName("priceLimitT1")]
        [Range(0, double.MaxValue)]
        public decimal? PriceLimitT1 { get; set; }
            
        [Description("Цена за тонну при количестве от PriceLimitT1")]
        [JsonPropertyName("priceT1")]
        [Range(0, double.MaxValue)]
        public decimal? PriceT1 { get; set; }
            
        [Description("Пороговое количество для цены T2 (в тоннах)")]
        [JsonPropertyName("priceLimitT2")]
        [Range(0, double.MaxValue)]
        public decimal? PriceLimitT2 { get; set; }
            
        [Description("Цена за тонну при количестве от PriceLimitT2")]
        [JsonPropertyName("priceT2")]
        [Range(0, double.MaxValue)]
        public decimal? PriceT2 { get; set; }
            
        [Description("Базовая цена за метр")]
        [JsonPropertyName("priceM")]
        [Range(0, double.MaxValue)]
        public decimal? PriceM { get; set; }
            
        [Description("Пороговое количество для цены M1 (в метрах)")]
        [JsonPropertyName("priceLimitM1")]
        [Range(0, double.MaxValue)]
        public decimal? PriceLimitM1 { get; set; }
            
        [Description("Цена за метр при количестве от PriceLimitM1")]
        [JsonPropertyName("priceM1")]
        [Range(0, double.MaxValue)]
        public decimal? PriceM1 { get; set; }
            
        [Description("Пороговое количество для цены M2 (в метрах)")]
        [JsonPropertyName("priceLimitM2")]
        [Range(0, double.MaxValue)]
        public decimal? PriceLimitM2 { get; set; }
            
        [Description("Цена за метр при количестве от PriceLimitM2")]
        [JsonPropertyName("priceM2")]
        [Range(0, double.MaxValue)]
        public decimal? PriceM2 { get; set; }
    }
}