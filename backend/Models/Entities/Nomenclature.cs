using System.ComponentModel.DataAnnotations;

namespace backend.Models.Entities
{
    public class Nomenclature
    {
        [Key]
        public int ID { get; set; }
        public int IDCat { get; set; }
        public int IDType { get; set; }
        public string IDTypeNew { get; set; } = string.Empty;
        public string ProductionType { get; set; } = string.Empty;
        public int? IDFunctionType { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Gost { get; set; } = string.Empty;
        public string FormOfLength { get; set; } = string.Empty;
        public string Manufacturer { get; set; } = string.Empty;
        public string SteelGrade { get; set; } = string.Empty;
        public decimal Diameter { get; set; }
        public decimal? ProfileSize2 { get; set; }
        public decimal PipeWallThickness { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal Koef { get; set; }
    }
}