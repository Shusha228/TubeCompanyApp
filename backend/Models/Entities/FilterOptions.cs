namespace backend.Models.Entities
{
    public class FilterOptions
    {
        public List<Stock> Warehouses { get; set; } = new();
        public List<string> Types { get; set; } = new();
        public List<decimal> Diameters { get; set; } = new();
        public List<decimal> WallThicknesses { get; set; } = new();
        public List<string> Standards { get; set; } = new();
        public List<string> SteelGrades { get; set; } = new();
    }
}