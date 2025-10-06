using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using backend.Models.Entities;
using backend.Data;
using System.ComponentModel.DataAnnotations;

namespace backend.Controllers
{
    /// <summary>
    /// Контроллер для управления ценами на трубы
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [SwaggerTag("Управление ценами на трубы - создание, чтение, обновление и удаление цен")]
    public class PricesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PricesController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// DTO для обновления цены
        /// </summary>
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

        [HttpGet("{productId}/{stockId}")]
        [SwaggerOperation(
            Summary = "Получить цену для конкретного товара и склада",
            Description = "Возвращает информацию о цене товара на указанном складе по ID товара и ID склада"
        )]
        [SwaggerResponse(200, "Цена найдена", typeof(Price))]
        [SwaggerResponse(404, "Цена не найдена")]
        public async Task<ActionResult<Price>> GetPrice(
            [SwaggerParameter("ID товара из номенклатуры", Required = true)] int productId,
            [SwaggerParameter("ID склада", Required = true)] int stockId)
        {
            var price = await _context.Prices
                .FirstOrDefaultAsync(p => p.ID == productId && p.IDStock == stockId);

            if (price == null)
            {
                return NotFound($"Price not found for product {productId} and stock {stockId}");
            }

            return Ok(price);
        }

        [HttpGet("product/{productId}")]
        [SwaggerOperation(
            Summary = "Получить все цены для конкретного товара",
            Description = "Возвращает список всех цен указанного товара на разных складах"
        )]
        [SwaggerResponse(200, "Список цен товара", typeof(List<Price>))]
        public async Task<ActionResult<List<Price>>> GetProductPrices(
            [SwaggerParameter("ID товара из номенклатуры", Required = true)] int productId)
        {
            var prices = await _context.Prices
                .Where(p => p.ID == productId)
                .ToListAsync();

            return Ok(prices);
        }

        [HttpGet("stock/{stockId}")]
        [SwaggerOperation(
            Summary = "Получить все цены на конкретном складе",
            Description = "Возвращает список цен всех товаров на указанном складе"
        )]
        [SwaggerResponse(200, "Список цен на складе", typeof(List<Price>))]
        public async Task<ActionResult<List<Price>>> GetStockPrices(
            [SwaggerParameter("ID склада", Required = true)] int stockId)
        {
            var prices = await _context.Prices
                .Where(p => p.IDStock == stockId)
                .ToListAsync();

            return Ok(prices);
        }

        [HttpPut("{productId}/{stockId}")]
        [SwaggerOperation(
            Summary = "Полное обновление цены для товара на складе",
            Description = "Обновляет все поля цены для указанного товара и склада"
        )]
        [SwaggerResponse(200, "Цена успешно обновлена", typeof(object))]
        [SwaggerResponse(400, "Неверные данные запроса")]
        [SwaggerResponse(404, "Цена не найдена")]
        [SwaggerResponse(500, "Ошибка при обновлении в базе данных")]
        public async Task<IActionResult> UpdatePrice(
            [SwaggerParameter("ID товара из номенклатуры", Required = true)] int productId,
            [SwaggerParameter("ID склада", Required = true)] int stockId,
            [SwaggerParameter("Данные для обновления цены", Required = true)] [FromBody] UpdatePriceRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var price = await _context.Prices
                .FirstOrDefaultAsync(p => p.ID == productId && p.IDStock == stockId);

            if (price == null)
            {
                return NotFound($"Price not found for product {productId} and stock {stockId}");
            }

            // Обновляем поля
            price.PriceT = request.PriceT;
            price.PriceLimitT1 = request.PriceLimitT1;
            price.PriceT1 = request.PriceT1;
            price.PriceLimitT2 = request.PriceLimitT2;
            price.PriceT2 = request.PriceT2;
            price.PriceM = request.PriceM;
            price.PriceLimitM1 = request.PriceLimitM1;
            price.PriceM1 = request.PriceM1;
            price.PriceLimitM2 = request.PriceLimitM2;
            price.PriceM2 = request.PriceM2;
            price.NDS = request.NDS;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { message = "Price updated successfully", price });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Error updating price: {ex.Message}");
            }
        }

        [HttpPatch("{productId}/{stockId}")]
        [SwaggerOperation(
            Summary = "Частичное обновление цены",
            Description = "Обновляет только указанные поля цены для товара на складе"
        )]
        [SwaggerResponse(200, "Цена успешно обновлена", typeof(object))]
        [SwaggerResponse(404, "Цена не найдена")]
        [SwaggerResponse(500, "Ошибка при обновлении в базе данных")]
        public async Task<IActionResult> PatchPrice(
            [SwaggerParameter("ID товара из номенклатуры", Required = true)] int productId,
            [SwaggerParameter("ID склада", Required = true)] int stockId,
            [SwaggerParameter("Словарь полей для обновления", Required = true)] [FromBody] Dictionary<string, object> updates)
        {
            var price = await _context.Prices
                .FirstOrDefaultAsync(p => p.ID == productId && p.IDStock == stockId);

            if (price == null)
            {
                return NotFound($"Price not found for product {productId} and stock {stockId}");
            }

            // Обновляем только переданные поля
            foreach (var update in updates)
            {
                var property = typeof(Price).GetProperty(update.Key);
                if (property != null && property.CanWrite)
                {
                    var value = Convert.ChangeType(update.Value, property.PropertyType);
                    property.SetValue(price, value);
                }
            }

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { message = "Price updated successfully", price });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Error updating price: {ex.Message}");
            }
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Создать новую цену для товара на складе",
            Description = "Создает новую запись цены для указанного товара и склада"
        )]
        [SwaggerResponse(201, "Цена успешно создана", typeof(Price))]
        [SwaggerResponse(400, "Неверные данные запроса")]
        [SwaggerResponse(409, "Цена уже существует")]
        [SwaggerResponse(500, "Ошибка при создании в базе данных")]
        public async Task<ActionResult<Price>> CreatePrice(
            [SwaggerParameter("Данные для создания цены", Required = true)] [FromBody] CreatePriceRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Проверяем существует ли уже цена для этого продукта и склада
            var existingPrice = await _context.Prices
                .FirstOrDefaultAsync(p => p.ID == request.ProductId && p.IDStock == request.StockId);

            if (existingPrice != null)
            {
                return Conflict($"Price already exists for product {request.ProductId} and stock {request.StockId}");
            }

            var price = new Price
            {
                ID = request.ProductId,
                IDStock = request.StockId,
                PriceT = request.PriceT,
                PriceLimitT1 = request.PriceLimitT1,
                PriceT1 = request.PriceT1,
                PriceLimitT2 = request.PriceLimitT2,
                PriceT2 = request.PriceT2,
                PriceM = request.PriceM,
                PriceLimitM1 = request.PriceLimitM1,
                PriceM1 = request.PriceM1,
                PriceLimitM2 = request.PriceLimitM2,
                PriceM2 = request.PriceM2,
                NDS = request.NDS
            };

            _context.Prices.Add(price);

            try
            {
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetPrice), new { productId = price.ID, stockId = price.IDStock }, price);
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Error creating price: {ex.Message}");
            }
        }

        /// <summary>
        /// DTO для создания новой цены
        /// </summary>
        public class CreatePriceRequest : UpdatePriceRequest
        {
            [SwaggerParameter("ID товара из номенклатуры", Required = true)]
            [Required]
            public int ProductId { get; set; }
            
            [SwaggerParameter("ID склада", Required = true)]
            [Required]
            public int StockId { get; set; }
        }
    }
}