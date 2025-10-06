using System.ComponentModel.DataAnnotations;

namespace backend.Models.Entities
{
    public class Remnant
    {
        [Key]
        public int ID { get; set; }
        
        [Key]
        public string IDStock { get; set; } = string.Empty;
        
        public decimal InStockT { get; set; }
        public decimal InStockM { get; set; }
        public decimal? SoonArriveT { get; set; }
        public decimal? SoonArriveM { get; set; }
        public decimal? ReservedT { get; set; }
        public decimal? ReservedM { get; set; }
        public decimal? AvgTubeLength { get; set; }
        public decimal? AvgTubeWeight { get; set; }
        
        // Навигационные свойства
        public Nomenclature? Nomenclature { get; set; }
        public Stock? Stock { get; set; }
    }
}