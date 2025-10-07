using System.ComponentModel.DataAnnotations;

namespace backend.Models.Entities
{
    public class UpdateLog
    {
        [Key]
        public int ID { get; set; }
        public string EntityType { get; set; } = string.Empty;
        public string EntityId { get; set; } = string.Empty;
        public string Operation { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string Details { get; set; } = string.Empty;
    }

    public class PriceUpdate
    {
        [Key]
        public int ID { get; set; }
        public int ProductId { get; set; }
        public string StockId { get; set; } = string.Empty;
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
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public bool IsProcessed { get; set; } = false;
    }

    public class RemnantUpdate
    {
        [Key]
        public int ID { get; set; }
        public int ProductId { get; set; }
        public string StockId { get; set; } = string.Empty;
        public decimal? InStockT { get; set; }
        public decimal? InStockM { get; set; }
        public decimal? SoonArriveT { get; set; }
        public decimal? SoonArriveM { get; set; }
        public decimal? ReservedT { get; set; }
        public decimal? ReservedM { get; set; }
        public decimal? AvgTubeLength { get; set; }
        public decimal? AvgTubeWeight { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public bool IsProcessed { get; set; } = false;
    }

    public class StockUpdate
    {
        [Key]
        public int ID { get; set; }
        public string StockId { get; set; } = string.Empty;
        public string? City { get; set; }
        public string? StockName { get; set; }
        public string? Address { get; set; }
        public string? Schedule { get; set; }
        public string? FIASId { get; set; }
        public string? OwnerInn { get; set; }
        public string? OwnerKpp { get; set; }
        public string? OwnerFullName { get; set; }
        public string? OwnerShortName { get; set; }
        public string? RailwayStation { get; set; }
        public string? ConsigneeCode { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public bool IsProcessed { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
    }

    public class UpdateStatus
    {
        public int PendingPriceUpdates { get; set; }
        public int PendingRemnantUpdates { get; set; }
        public int PendingStockUpdates { get; set; }
        public int TotalPending { get; set; }
        public DateTime? LastProcessedTime { get; set; }
    }
}