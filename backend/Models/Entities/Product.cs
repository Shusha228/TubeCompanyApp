namespace backend.Models.Entities
{
    public class Product
    {
        public int ID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public decimal Diameter { get; set; }
        public decimal WallThickness { get; set; }
        public string Standard { get; set; } = string.Empty;
        public string SteelGrade { get; set; } = string.Empty;
        public string Warehouse { get; set; } = string.Empty;
        public string WarehouseName { get; set; } = string.Empty;
        public decimal PricePerMeter { get; set; }
        public decimal PricePerTon { get; set; }
        public decimal StockMeters { get; set; }
        public decimal StockTons { get; set; }
        public decimal? PriceLimitM1 { get; set; }
        public decimal? PriceM1 { get; set; }
        public decimal? PriceLimitM2 { get; set; }
        public decimal? PriceM2 { get; set; }
        public decimal? PriceLimitT1 { get; set; }
        public decimal? PriceT1 { get; set; }
        public decimal? PriceLimitT2 { get; set; }
        public decimal? PriceT2 { get; set; }
        public decimal Koef { get; set; }
        public decimal? AvgTubeLength { get; set; }
        public decimal? AvgTubeWeight { get; set; }
    }

    public class ProductFilter
    {
        public string? WarehouseId { get; set; }
        public string? Type { get; set; }
        public decimal? Diameter { get; set; }
        public decimal? WallThickness { get; set; }
        public string? Standard { get; set; }
        public string? SteelGrade { get; set; }
    }
}