using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using backend.Models.Entities;
using backend.Data;
using System.ComponentModel.DataAnnotations;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [SwaggerTag("Управление ценами на трубы")]
    public class PricesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PricesController> _logger;

        public PricesController(ApplicationDbContext context, ILogger<PricesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// DTO для создания и обновления цены
        /// </summary>
        public class PriceRequest
        {
            [Required]
            [Range(1, int.MaxValue)]
            public int ProductId { get; set; }
            
            [Required]
            [StringLength(36, MinimumLength = 36)]
            public string StockId { get; set; } = string.Empty;
            
            [Required]
            [Range(0, double.MaxValue)]
            public decimal PriceT { get; set; }
            
            [Range(0, double.MaxValue)]
            public decimal? PriceLimitT1 { get; set; }
            
            [Range(0, double.MaxValue)]
            public decimal? PriceT1 { get; set; }
            
            [Range(0, double.MaxValue)]
            public decimal? PriceLimitT2 { get; set; }
            
            [Range(0, double.MaxValue)]
            public decimal? PriceT2 { get; set; }
            
            [Required]
            [Range(0, double.MaxValue)]
            public decimal PriceM { get; set; }
            
            [Range(0, double.MaxValue)]
            public decimal? PriceLimitM1 { get; set; }
            
            [Range(0, double.MaxValue)]
            public decimal? PriceM1 { get; set; }
            
            [Range(0, double.MaxValue)]
            public decimal? PriceLimitM2 { get; set; }
            
            [Range(0, double.MaxValue)]
            public decimal? PriceM2 { get; set; }
            
            [Required]
            [Range(0, 100)]
            public decimal NDS { get; set; }
        }

        /// <summary>
        /// DTO для частичного обновления
        /// </summary>
        public class PricePatchRequest
        {
            [Range(0, double.MaxValue)]
            public decimal? PriceT { get; set; }
            
            [Range(0, double.MaxValue)]
            public decimal? PriceLimitT1 { get; set; }
            
            [Range(0, double.MaxValue)]
            public decimal? PriceT1 { get; set; }
            
            [Range(0, double.MaxValue)]
            public decimal? PriceLimitT2 { get; set; }
            
            [Range(0, double.MaxValue)]
            public decimal? PriceT2 { get; set; }
            
            [Range(0, double.MaxValue)]
            public decimal? PriceM { get; set; }
            
            [Range(0, double.MaxValue)]
            public decimal? PriceLimitM1 { get; set; }
            
            [Range(0, double.MaxValue)]
            public decimal? PriceM1 { get; set; }
            
            [Range(0, double.MaxValue)]
            public decimal? PriceLimitM2 { get; set; }
            
            [Range(0, double.MaxValue)]
            public decimal? PriceM2 { get; set; }
            
            [Range(0, 100)]
            public decimal? NDS { get; set; }
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Получить все цены", Description = "Возвращает список всех цен с пагинацией")]
        [SwaggerResponse(200, "Успешный запрос", typeof(List<Price>))]
        public async Task<ActionResult<List<Price>>> GetAllPrices(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 50)
        {
            try
            {
                var prices = await _context.Prices
                    .OrderBy(p => p.ID)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                _logger.LogInformation("Retrieved {Count} prices", prices.Count);
                return Ok(prices);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all prices");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{productId}/{stockId}")]
        [SwaggerOperation(Summary = "Получить цену по ID товара и склада")]
        [SwaggerResponse(200, "Цена найдена", typeof(Price))]
        [SwaggerResponse(404, "Цена не найдена")]
        public async Task<ActionResult<Price>> GetPrice(int productId, string stockId)
        {
            try
            {
                var price = await _context.Prices
                    .FirstOrDefaultAsync(p => p.ID == productId && p.IDStock == stockId);

                if (price == null)
                {
                    _logger.LogWarning("Price not found for product {ProductId} and stock {StockId}", productId, stockId);
                    return NotFound(new { message = $"Price not found for product {productId} and stock {stockId}" });
                }

                return Ok(price);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting price for product {ProductId} and stock {StockId}", productId, stockId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("product/{productId}")]
        [SwaggerOperation(Summary = "Получить все цены товара")]
        [SwaggerResponse(200, "Успешный запрос", typeof(List<Price>))]
        public async Task<ActionResult<List<Price>>> GetProductPrices(int productId)
        {
            try
            {
                var prices = await _context.Prices
                    .Where(p => p.ID == productId)
                    .ToListAsync();

                _logger.LogInformation("Found {Count} prices for product {ProductId}", prices.Count, productId);
                return Ok(prices);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting prices for product {ProductId}", productId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("stock/{stockId}")]
        [SwaggerOperation(Summary = "Получить все цены на складе")]
        [SwaggerResponse(200, "Успешный запрос", typeof(List<Price>))]
        public async Task<ActionResult<List<Price>>> GetStockPrices(string stockId)
        {
            try
            {
                var prices = await _context.Prices
                    .Where(p => p.IDStock == stockId)
                    .ToListAsync();

                _logger.LogInformation("Found {Count} prices for stock {StockId}", prices.Count, stockId);
                return Ok(prices);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting prices for stock {StockId}", stockId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Создать новую цену")]
        [SwaggerResponse(201, "Цена создана", typeof(Price))]
        [SwaggerResponse(400, "Неверные данные")]
        [SwaggerResponse(409, "Цена уже существует")]
        public async Task<ActionResult<Price>> CreatePrice([FromBody] PriceRequest request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for create price: {Errors}", 
                    string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                return BadRequest(ModelState);
            }

            try
            {
                // Проверяем существование связанных сущностей
                var productExists = await _context.Nomenclatures.AnyAsync(n => n.ID == request.ProductId);
                if (!productExists)
                {
                    return BadRequest(new { message = $"Product with ID {request.ProductId} not found" });
                }

                var stockExists = await _context.Stocks.AnyAsync(s => s.IDStock == request.StockId);
                if (!stockExists)
                {
                    return BadRequest(new { message = $"Stock with ID {request.StockId} not found" });
                }

                // Проверяем существование цены
                var existingPrice = await _context.Prices
                    .FirstOrDefaultAsync(p => p.ID == request.ProductId && p.IDStock == request.StockId);

                if (existingPrice != null)
                {
                    return Conflict(new { message = $"Price already exists for product {request.ProductId} and stock {request.StockId}" });
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
                await _context.SaveChangesAsync();

                _logger.LogInformation("Created price for product {ProductId} and stock {StockId}", request.ProductId, request.StockId);
                return CreatedAtAction(nameof(GetPrice), new { productId = price.ID, stockId = price.IDStock }, price);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error creating price");
                return StatusCode(500, new { message = "Error creating price in database" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error creating price");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPut("{productId}/{stockId}")]
        [SwaggerOperation(Summary = "Полное обновление цены")]
        [SwaggerResponse(200, "Цена обновлена", typeof(Price))]
        [SwaggerResponse(400, "Неверные данные")]
        [SwaggerResponse(404, "Цена не найдена")]
        public async Task<ActionResult<Price>> UpdatePrice(int productId, string stockId, [FromBody] PriceRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var price = await _context.Prices
                    .FirstOrDefaultAsync(p => p.ID == productId && p.IDStock == stockId);

                if (price == null)
                {
                    return NotFound(new { message = $"Price not found for product {productId} and stock {stockId}" });
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

                await _context.SaveChangesAsync();

                _logger.LogInformation("Updated price for product {ProductId} and stock {StockId}", productId, stockId);
                return Ok(price);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error updating price");
                return StatusCode(500, new { message = "Error updating price in database" });
            }
        }

        [HttpPatch("{productId}/{stockId}")]
        [SwaggerOperation(Summary = "Частичное обновление цены")]
        [SwaggerResponse(200, "Цена обновлена", typeof(Price))]
        [SwaggerResponse(400, "Неверные данные")]
        [SwaggerResponse(404, "Цена не найдена")]
        public async Task<ActionResult<Price>> PatchPrice(int productId, string stockId, [FromBody] PricePatchRequest request)
        {
            try
            {
                var price = await _context.Prices
                    .FirstOrDefaultAsync(p => p.ID == productId && p.IDStock == stockId);

                if (price == null)
                {
                    return NotFound(new { message = $"Price not found for product {productId} and stock {stockId}" });
                }

                // Обновляем только переданные поля
                if (request.PriceT.HasValue) price.PriceT = request.PriceT.Value;
                if (request.PriceLimitT1.HasValue) price.PriceLimitT1 = request.PriceLimitT1.Value;
                if (request.PriceT1.HasValue) price.PriceT1 = request.PriceT1.Value;
                if (request.PriceLimitT2.HasValue) price.PriceLimitT2 = request.PriceLimitT2.Value;
                if (request.PriceT2.HasValue) price.PriceT2 = request.PriceT2.Value;
                if (request.PriceM.HasValue) price.PriceM = request.PriceM.Value;
                if (request.PriceLimitM1.HasValue) price.PriceLimitM1 = request.PriceLimitM1.Value;
                if (request.PriceM1.HasValue) price.PriceM1 = request.PriceM1.Value;
                if (request.PriceLimitM2.HasValue) price.PriceLimitM2 = request.PriceLimitM2.Value;
                if (request.PriceM2.HasValue) price.PriceM2 = request.PriceM2.Value;
                if (request.NDS.HasValue) price.NDS = request.NDS.Value;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Patched price for product {ProductId} and stock {StockId}", productId, stockId);
                return Ok(price);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error patching price");
                return StatusCode(500, new { message = "Error updating price in database" });
            }
        }

        [HttpDelete("{productId}/{stockId}")]
        [SwaggerOperation(Summary = "Удалить цену")]
        [SwaggerResponse(200, "Цена удалена")]
        [SwaggerResponse(404, "Цена не найдена")]
        public async Task<IActionResult> DeletePrice(int productId, string stockId)
        {
            try
            {
                var price = await _context.Prices
                    .FirstOrDefaultAsync(p => p.ID == productId && p.IDStock == stockId);

                if (price == null)
                {
                    return NotFound(new { message = $"Price not found for product {productId} and stock {stockId}" });
                }

                _context.Prices.Remove(price);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Deleted price for product {ProductId} and stock {StockId}", productId, stockId);
                return Ok(new { message = "Price deleted successfully" });
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error deleting price");
                return StatusCode(500, new { message = "Error deleting price from database" });
            }
        }

        [HttpGet("debug")]
        [SwaggerOperation(Summary = "Отладочная информация", Description = "Возвращает отладочную информацию о ценах")]
        public async Task<ActionResult> Debug()
        {
            try
            {
                var pricesCount = await _context.Prices.CountAsync();
                var stocksCount = await _context.Stocks.CountAsync();
                var productsCount = await _context.Nomenclatures.CountAsync();

                var samplePrices = await _context.Prices
                    .Take(5)
                    .Select(p => new { p.ID, p.IDStock, p.PriceT, p.PriceM })
                    .ToListAsync();

                return Ok(new
                {
                    PricesCount = pricesCount,
                    StocksCount = stocksCount,
                    ProductsCount = productsCount,
                    SamplePrices = samplePrices,
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in debug endpoint");
                return StatusCode(500, new { message = "Error getting debug information" });
            }
        }
    }
}