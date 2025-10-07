using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace backend.Models.DTOs.Price
{
    public class CreatePriceRequest : UpdatePriceRequest
    {
        [SwaggerParameter("ID товара из номенклатуры", Required = true)]
        [Required]
        public int ProductId { get; set; }

        [SwaggerParameter("ID склада", Required = true)]
        [Required]
        public string StockId { get; set; } = string.Empty;
    }
}