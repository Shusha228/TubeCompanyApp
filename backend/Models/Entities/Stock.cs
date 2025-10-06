using System.ComponentModel.DataAnnotations;

namespace backend.Models.Entities
{
    public class Stock
    {
        [Key]
        public string IDStock { get; set; } = string.Empty; // GUID как строка
        
        public string City { get; set; } = string.Empty;
        public string StockName { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? Schedule { get; set; }
        public string? FIASId { get; set; }
        public string? OwnerInn { get; set; }
        public string? OwnerKpp { get; set; }
        public string? OwnerFullName { get; set; }
        public string? OwnerShortName { get; set; }
        public string? RailwayStation { get; set; }
        public string? ConsigneeCode { get; set; }
    }
}