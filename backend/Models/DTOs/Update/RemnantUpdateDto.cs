using System.ComponentModel;
using System.Text.Json.Serialization;

namespace backend.Models.DTOs.Update
{
    public class RemnantUpdateDto
    {
        [Description("Идентификатор номенклатуры")]
        [JsonPropertyName("ID")]
        public int ID { get; set; }
        
        [Description("Идентификатор склада")]
        [JsonPropertyName("IDStock")]
        public string IDStock { get; set; } = string.Empty;
        
        [Description("Остаток на складе в тоннах")]
        public decimal? InStockT { get; set; }
        
        [Description("Остаток на складе в метрах")]
        public decimal? InStockM { get; set; }
        
        [Description("Ожидаемое поступление в тоннах")]
        public decimal? SoonArriveT { get; set; }
        
        [Description("Ожидаемое поступление в метрах")]
        public decimal? SoonArriveM { get; set; }
        
        [Description("Зарезервировано в тоннах")]
        public decimal? ReservedT { get; set; }
        
        [Description("Зарезервировано в метрах")]
        public decimal? ReservedM { get; set; }
        
        [Description("Доступно под заказ")]
        public bool UnderTheOrder { get; set; }
        
        [Description("Средняя длина трубы")]
        public decimal? AvgTubeLength { get; set; }
        
        [Description("Средний вес трубы")]
        public decimal? AvgTubeWeight { get; set; }
    }
}