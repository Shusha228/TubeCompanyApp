using System.Text.Json.Serialization;
namespace backend.Models.DTOs.Update
{
    public class RemnantUpdateDto
    {
        [JsonPropertyName("ID")]
        public int ID { get; set; }
        
        [JsonPropertyName("IDStock")]
        public string IDStock { get; set; } = string.Empty;
        
        public decimal? InStockT { get; set; }
        public decimal? InStockM { get; set; }
        public decimal? SoonArriveT { get; set; }
        public decimal? SoonArriveM { get; set; }
        public decimal? ReservedT { get; set; }
        public decimal? ReservedM { get; set; }
        public bool UnderTheOrder { get; set; }
        public decimal? AvgTubeLength { get; set; }
        public decimal? AvgTubeWeight { get; set; }
    }
}