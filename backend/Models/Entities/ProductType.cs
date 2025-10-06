using System.ComponentModel.DataAnnotations;

namespace backend.Models.Entities
{
    public class ProductType
    {
        [Key]
        public int IDType { get; set; }
        public string Type { get; set; } = string.Empty;
        public int? IDParentType { get; set; }
    }
}