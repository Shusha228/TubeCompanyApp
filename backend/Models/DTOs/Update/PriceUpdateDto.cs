using System.ComponentModel;
using System.Text.Json.Serialization;

namespace backend.Models.DTOs.Update
{
    public class PriceUpdateDto
    {
        [Description("Идентификатор номенклатуры")]
        [JsonPropertyName("ID")]
        public int ID { get; set; }
        
        [Description("Идентификатор склада")]
        [JsonPropertyName("IDStock")] 
        public string IDStock { get; set; } = string.Empty;
        
        [Description("Цена за тонну (базовая)")]
        public decimal? PriceT { get; set; }
        
        [Description("Цена за тонну (скидка 1 уровень)")]
        public decimal? PriceT1 { get; set; }
        
        [Description("Цена за тонну (скидка 2 уровень)")]
        public decimal? PriceT2 { get; set; }
        
        [Description("Цена за метр (базовая)")]
        public decimal? PriceM { get; set; }
        
        [Description("Цена за метр (скидка 1 уровень)")]
        public decimal? PriceM1 { get; set; }
        
        [Description("Цена за метр (скидка 2 уровень)")]
        public decimal? PriceM2 { get; set; }
        
        [Description("Порог количества для скидки T1 (тонны)")]
        public decimal? PriceLimitT1 { get; set; }
        
        [Description("Порог количества для скидки T2 (тонны)")]
        public decimal? PriceLimitT2 { get; set; }
        
        [Description("Порог количества для скидки M1 (метры)")]
        public decimal? PriceLimitM1 { get; set; }
        
        [Description("Порог количества для скидки M2 (метры)")]
        public decimal? PriceLimitM2 { get; set; }
        
        [Description("НДС в процентах")]
        public decimal? NDS { get; set; }
    }
}