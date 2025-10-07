using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace backend.Models.DTOs.Price
{
    public class PricePatchRequest
    {
        [JsonPropertyName("nds")]
        [Range(0, 100)]
        public decimal? NDS { get; set; }
            
        [JsonPropertyName("priceT")]
        [Range(0, double.MaxValue)]
        public decimal? PriceT { get; set; }
            
        [JsonPropertyName("priceLimitT1")]
        [Range(0, double.MaxValue)]
        public decimal? PriceLimitT1 { get; set; }
            
        [JsonPropertyName("priceT1")]
        [Range(0, double.MaxValue)]
        public decimal? PriceT1 { get; set; }
            
        [JsonPropertyName("priceLimitT2")]
        [Range(0, double.MaxValue)]
        public decimal? PriceLimitT2 { get; set; }
            
        [JsonPropertyName("priceT2")]
        [Range(0, double.MaxValue)]
        public decimal? PriceT2 { get; set; }
            
        [JsonPropertyName("priceM")]
        [Range(0, double.MaxValue)]
        public decimal? PriceM { get; set; }
            
        [JsonPropertyName("priceLimitM1")]
        [Range(0, double.MaxValue)]
        public decimal? PriceLimitM1 { get; set; }
            
        [JsonPropertyName("priceM1")]
        [Range(0, double.MaxValue)]
        public decimal? PriceM1 { get; set; }
            
        [JsonPropertyName("priceLimitM2")]
        [Range(0, double.MaxValue)]
        public decimal? PriceLimitM2 { get; set; }
            
        [JsonPropertyName("priceM2")]
        [Range(0, double.MaxValue)]
        public decimal? PriceM2 { get; set; }
    }
}