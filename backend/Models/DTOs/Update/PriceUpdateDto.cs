using System.Text.Json.Serialization;
namespace backend.Models.DTOs.Update
{
    public class PriceUpdateDto
    {
        [JsonPropertyName("ID")]
        public int ID { get; set; }
        
        [JsonPropertyName("IDStock")] 
        public string IDStock { get; set; } = string.Empty;
        
        public decimal? PriceT { get; set; }
        public decimal? PriceT1 { get; set; }
        public decimal? PriceT2 { get; set; }
        public decimal? PriceM { get; set; }
        public decimal? PriceM1 { get; set; }
        public decimal? PriceM2 { get; set; }
        public decimal? PriceLimitT1 { get; set; }
        public decimal? PriceLimitT2 { get; set; }
        public decimal? PriceLimitM1 { get; set; }
        public decimal? PriceLimitM2 { get; set; }
        public decimal? NDS { get; set; }
    }

}