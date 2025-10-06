using System.ComponentModel.DataAnnotations;

namespace backend.Models.Entities
{
    public class Remnant
    {
        [Key]
        public int ID { get; set; }
        public int IDStock { get; set; }
        public decimal InStockT { get; set; }
        public decimal InStockM { get; set; }
        public decimal? SoonArriveT { get; set; }
        public decimal? SoonArriveM { get; set; }
        public decimal? ReservedT { get; set; }
        public decimal? ReservedM { get; set; }
        public bool UnderTheOrder { get; set; }
        public decimal AvgTubeLength { get; set; }
        public decimal AvgTubeWeight { get; set; }
    }
}