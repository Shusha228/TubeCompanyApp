namespace backend.Models
{
    public class ProductType
    {
        public int IDType { get; set; }
        public string Type { get; set; } = string.Empty;
        public int? IDParentType { get; set; }
    }

    public class Nomenclature
    {
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

    public class Price
    {
        public int ID { get; set; }
        public int IDStock { get; set; }
        public decimal PriceT { get; set; }
        public decimal? PriceLimitT1 { get; set; }
        public decimal? PriceT1 { get; set; }
        public decimal? PriceLimitT2 { get; set; }
        public decimal? PriceT2 { get; set; }
        public decimal PriceM { get; set; }
        public decimal? PriceLimitM1 { get; set; }
        public decimal? PriceM1 { get; set; }
        public decimal? PriceLimitM2 { get; set; }
        public decimal? PriceM2 { get; set; }
        public decimal NDS { get; set; }
    }

    public class Remnant
    {
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

    public class Stock
    {
        public int IDStock { get; set; }
        public string City { get; set; } = string.Empty;
        public string StockName { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? Schedule { get; set; }
        public int? IDDivision { get; set; }
        public bool? CashPayment { get; set; }
        public bool? CardPayment { get; set; }
        public string? FIASId { get; set; }
        public string? OwnerInn { get; set; }
        public string? OwnerKpp { get; set; }
        public string? OwnerFullName { get; set; }
        public string? OwnerShortName { get; set; }
        public string? RailwayStation { get; set; }
        public string? ConsigneeCode { get; set; }
    }

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
        public decimal AvgTubeLength { get; set; }
        public decimal AvgTubeWeight { get; set; }
    }

    public class ProductFilter
    {
        public int? WarehouseId { get; set; }
        public string? Type { get; set; }
        public decimal? Diameter { get; set; }
        public decimal? WallThickness { get; set; }
        public string? Standard { get; set; }
        public string? SteelGrade { get; set; }
    }

    public class CartItem
    {
        public int ProductId { get; set; }
        public Product Product { get; set; } = new();
        public decimal Quantity { get; set; }
        public bool IsInMeters { get; set; } = true;
        public decimal FinalPrice { get; set; }
    }

    public class Order
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public long TelegramUserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Inn { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<CartItem> Items { get; set; } = new();
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Pending";
    }

    public class CreateOrderRequest
    {
        public long TelegramUserId { get; set; }
        public List<CartItem> Items { get; set; } = new();
        public CustomerInfo CustomerInfo { get; set; } = new();
    }

    public class CustomerInfo
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Inn { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class PriceCalculationRequest
    {
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
        public bool IsInMeters { get; set; } = true;
    }

    public class PriceCalculationResponse
    {
        public decimal FinalPrice { get; set; }
        public decimal BasePrice { get; set; }
        public decimal DiscountPercent { get; set; }
    }

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