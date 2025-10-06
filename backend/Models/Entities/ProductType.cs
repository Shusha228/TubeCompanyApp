using System.ComponentModel.DataAnnotations;

namespace backend.Models.Entities
{
    public class ProductType
    {
        [Key]
        public int IDType { get; set; } // Изменяем на int
        public string Type { get; set; } = string.Empty;
        public string? IDParentType { get; set; } // Оставляем string для GUID
        public string OriginalGuid { get; set; } = string.Empty; // Добавляем поле для хранения оригинального GUID
    }
}