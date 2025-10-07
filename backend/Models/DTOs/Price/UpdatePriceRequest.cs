
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;


namespace backend.Models.DTOs.Price
{
    public class UpdatePriceRequest
        {
            [SwaggerParameter("Цена за тонну на базовом пороге", Required = true)]
            [Required]
            [Range(0, double.MaxValue)]
            public decimal PriceT { get; set; }

            [SwaggerParameter("Порог объема в тоннах для цены PriceT1")]
            [Range(0, double.MaxValue)]
            public decimal? PriceLimitT1 { get; set; }

            [SwaggerParameter("Цена за тонну при достижении PriceLimitT1")]
            [Range(0, double.MaxValue)]
            public decimal? PriceT1 { get; set; }

            [SwaggerParameter("Порог объема в тоннах для цены PriceT2")]
            [Range(0, double.MaxValue)]
            public decimal? PriceLimitT2 { get; set; }

            [SwaggerParameter("Цена за тонну при достижении PriceLimitT2")]
            [Range(0, double.MaxValue)]
            public decimal? PriceT2 { get; set; }

            [SwaggerParameter("Цена за метр на базовом пороге", Required = true)]
            [Required]
            [Range(0, double.MaxValue)]
            public decimal PriceM { get; set; }

            [SwaggerParameter("Порог объема в метрах для PriceM1")]
            [Range(0, double.MaxValue)]
            public decimal? PriceLimitM1 { get; set; }

            [SwaggerParameter("Цена за метр при достижении PriceLimitM1")]
            [Range(0, double.MaxValue)]
            public decimal? PriceM1 { get; set; }

            [SwaggerParameter("Порог объема в метрах для PriceM2")]
            [Range(0, double.MaxValue)]
            public decimal? PriceLimitM2 { get; set; }

            [SwaggerParameter("Цена за метр при достижении PriceLimitM2")]
            [Range(0, double.MaxValue)]
            public decimal? PriceM2 { get; set; }

            [SwaggerParameter("Ставка НДС в процентах", Required = true)]
            [Required]
            [Range(0, 100)]
            public decimal NDS { get; set; }
        }
}