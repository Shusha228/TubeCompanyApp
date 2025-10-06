using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using backend.Models.Entities;
using backend.Data;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;

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
        private readonly ILogger<PricesController> _logger; // ДОБАВЛЕНО: поле для логгера

        // ДОБАВЛЕНО: logger в конструктор
        public PricesController(ApplicationDbContext context, ILogger<PricesController> logger)
        {
            _context = context;
            _logger = logger; // Инициализация логгера
        }
        
        public class PaginatedResponse<T>
        {
            [JsonPropertyName("data")]
            public List<T> Data { get; set; } = new List<T>();

            [JsonPropertyName("page")]
            public int Page { get; set; }

            [JsonPropertyName("pageSize")]
            public int PageSize { get; set; }

            [JsonPropertyName("totalCount")]
            public int TotalCount { get; set; }

            [JsonPropertyName("totalPages")]
            public int TotalPages { get; set; }

            [JsonPropertyName("hasNextPage")]
            public bool HasNextPage { get; set; }

            [JsonPropertyName("hasPreviousPage")]
            public bool HasPreviousPage { get; set; }
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
            [SwaggerParameter("ID склада", Required = true)] string stockId)
        {
            _logger.LogInformation("Getting price for product {ProductId} and stock {StockId}", productId, stockId);
            
            var price = await _context.Prices
                .FirstOrDefaultAsync(p => p.ID == productId && p.IDStock == stockId);

            if (price == null)
            {
                _logger.LogWarning("Price not found for product {ProductId} and stock {StockId}", productId, stockId);
                return NotFound($"Price not found for product {productId} and stock {stockId}");
            }

            _logger.LogInformation("Price found for product {ProductId} and stock {StockId}", productId, stockId);
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
            _logger.LogInformation("Getting all prices for product {ProductId}", productId);
            
            var prices = await _context.Prices
                .Where(p => p.ID == productId)
                .ToListAsync();

            _logger.LogInformation("Found {Count} prices for product {ProductId}", prices.Count, productId);
            return Ok(prices);
        }

        [HttpGet("stock/{stockId}")]
        [SwaggerOperation(
            Summary = "Получить цены на конкретном складе по диапазону товаров",
            Description = "Возвращает список цен товаров на указанном складе по диапазону ID товаров с пагинацией"
        )]
        [SwaggerResponse(200, "Список цен на складе с мета-информацией", typeof(PaginatedResult<Price>))]
        public async Task<ActionResult<PaginatedResult<Price>>> GetStockPrices(
            [SwaggerParameter("ID склада", Required = true)] string stockId,
            [SwaggerParameter("Начальный ID товара (включительно)")] [FromQuery] int? fromId = null,
            [SwaggerParameter("Конечный ID товара (включительно)")] [FromQuery] int? toId = null,
            [SwaggerParameter("Лимит количества записей на странице")] [FromQuery] int pageLimit = 20,
            [SwaggerParameter("Номер страницы (начиная с 1)")] [FromQuery] int page = 1)
        {
            _logger.LogInformation("Getting prices for stock {StockId} - FromId: {FromId}, ToId: {ToId}, Page: {Page}, PageLimit: {PageLimit}", 
                stockId, fromId?.ToString() ?? "all", toId?.ToString() ?? "all", page, pageLimit);

            try
            {
                // Валидация параметров
                if (page < 1)
                {
                    return BadRequest("Page must be greater than 0");
                }

                if (pageLimit < 1 || pageLimit > 100)
                {
                    return BadRequest("PageLimit must be between 1 and 100");
                }

                var query = _context.Prices
                    .Where(p => p.IDStock == stockId);

                // Фильтрация по диапазону ID
                if (fromId.HasValue)
                {
                    query = query.Where(p => p.ID >= fromId.Value);
                }

                if (toId.HasValue)
                {
                    query = query.Where(p => p.ID <= toId.Value);
                }

                // Получаем общее количество записей для мета-информации
                var totalCount = await query.CountAsync();

                // Сортировка по ID для предсказуемого результата
                query = query.OrderBy(p => p.ID);

                // Применяем пагинацию
                var prices = await query
                    .Skip((page - 1) * pageLimit)
                    .Take(pageLimit)
                    .ToListAsync();

                // Рассчитываем общее количество страниц
                var totalPages = (int)Math.Ceiling(totalCount / (double)pageLimit);

                _logger.LogInformation("Returning {Count} prices for stock {StockId} (page {Page} of {TotalPages})", 
                    prices.Count, stockId, page, totalPages);

                // Создаем ответ в нужном формате
                var result = new PaginatedResult<Price>
                {
                    Data = prices,
                    Meta = new Meta
                    {
                        TotalPages = totalPages,
                        Page = page,
                        PageLimit = pageLimit,
                        TotalCount = totalCount
                    }
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving prices for stock {StockId}", stockId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Мета-информация о пагинации
        /// </summary>
        public class Meta
        {
            [JsonPropertyName("totalPages")]
            public int TotalPages { get; set; }

            [JsonPropertyName("page")]
            public int Page { get; set; }

            [JsonPropertyName("pageLimit")]
            public int PageLimit { get; set; }

            [JsonPropertyName("totalCount")]
            public int TotalCount { get; set; }
        }

        /// <summary>
        /// Результат с пагинацией
        /// </summary>
        public class PaginatedResult<T>
        {
            [JsonPropertyName("data")]
            public IEnumerable<T> Data { get; set; } = new List<T>();

            [JsonPropertyName("meta")]
            public Meta Meta { get; set; } = new Meta();
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
            [SwaggerParameter("ID склада", Required = true)] string stockId,
            [SwaggerParameter("Данные для обновления цены", Required = true)] [FromBody] UpdatePriceRequest request)
        {
            _logger.LogInformation("Starting full update for product {ProductId} and stock {StockId}", productId, stockId);
            
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for product {ProductId} and stock {StockId}", productId, stockId);
                return BadRequest(ModelState);
            }

            var price = await _context.Prices
                .FirstOrDefaultAsync(p => p.ID == productId && p.IDStock == stockId);

            if (price == null)
            {
                _logger.LogWarning("Price not found for full update - product {ProductId} and stock {StockId}", productId, stockId);
                return NotFound($"Price not found for product {productId} and stock {stockId}");
            }

            // Логируем текущие значения перед обновлением
            _logger.LogInformation("Before full update - Product: {ProductId}, Stock: {StockId}, NDS: {CurrentNDS}, PriceT: {CurrentPriceT}, PriceM: {CurrentPriceM}", 
                productId, stockId, price.NDS, price.PriceT, price.PriceM);

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
                _logger.LogInformation("Successfully updated price for product {ProductId} and stock {StockId}", productId, stockId);
                return Ok(new { message = "Price updated successfully", price });
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error updating price for product {ProductId} and stock {StockId}", productId, stockId);
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
            [SwaggerParameter("ID склада", Required = true)] string stockId,
            [SwaggerParameter("Данные для частичного обновления", Required = true)] 
            [FromBody] PricePatchRequest request)
        {
            _logger.LogInformation("Starting patch update for product {ProductId} and stock {StockId}", productId, stockId);
            
            try
            {
                var price = await _context.Prices
                    .FirstOrDefaultAsync(p => p.ID == productId && p.IDStock == stockId);

                if (price == null)
                {
                    _logger.LogWarning("Price not found for patch update - product {ProductId} and stock {StockId}", productId, stockId);
                    return NotFound(new { message = $"Price not found for product {productId} and stock {stockId}" });
                }

                // Логируем текущие значения перед обновлением
                _logger.LogInformation("Before patch update - Product: {ProductId}, Stock: {StockId}, NDS: {CurrentNDS}, PriceT: {CurrentPriceT}, PriceM: {CurrentPriceM}", 
                    productId, stockId, price.NDS, price.PriceT, price.PriceM);

                // Обновляем только переданные поля
                if (request.NDS.HasValue) 
                {
                    price.NDS = request.NDS.Value;
                    _logger.LogInformation("Updated NDS to: {NewNDS}", request.NDS.Value);
                }
                
                if (request.PriceT.HasValue) 
                {
                    price.PriceT = request.PriceT.Value;
                    _logger.LogInformation("Updated PriceT to: {NewPriceT}", request.PriceT.Value);
                }
                
                if (request.PriceM.HasValue) 
                {
                    price.PriceM = request.PriceM.Value;
                    _logger.LogInformation("Updated PriceM to: {NewPriceM}", request.PriceM.Value);
                }
                
                if (request.PriceLimitT1.HasValue) 
                {
                    price.PriceLimitT1 = request.PriceLimitT1.Value;
                    _logger.LogInformation("Updated PriceLimitT1 to: {NewValue}", request.PriceLimitT1.Value);
                }
                
                if (request.PriceT1.HasValue) 
                {
                    price.PriceT1 = request.PriceT1.Value;
                    _logger.LogInformation("Updated PriceT1 to: {NewValue}", request.PriceT1.Value);
                }
                
                if (request.PriceLimitT2.HasValue) 
                {
                    price.PriceLimitT2 = request.PriceLimitT2.Value;
                    _logger.LogInformation("Updated PriceLimitT2 to: {NewValue}", request.PriceLimitT2.Value);
                }
                
                if (request.PriceT2.HasValue) 
                {
                    price.PriceT2 = request.PriceT2.Value;
                    _logger.LogInformation("Updated PriceT2 to: {NewValue}", request.PriceT2.Value);
                }
                
                if (request.PriceLimitM1.HasValue) 
                {
                    price.PriceLimitM1 = request.PriceLimitM1.Value;
                    _logger.LogInformation("Updated PriceLimitM1 to: {NewValue}", request.PriceLimitM1.Value);
                }
                
                if (request.PriceM1.HasValue) 
                {
                    price.PriceM1 = request.PriceM1.Value;
                    _logger.LogInformation("Updated PriceM1 to: {NewValue}", request.PriceM1.Value);
                }
                
                if (request.PriceLimitM2.HasValue) 
                {
                    price.PriceLimitM2 = request.PriceLimitM2.Value;
                    _logger.LogInformation("Updated PriceLimitM2 to: {NewValue}", request.PriceLimitM2.Value);
                }
                
                if (request.PriceM2.HasValue) 
                {
                    price.PriceM2 = request.PriceM2.Value;
                    _logger.LogInformation("Updated PriceM2 to: {NewValue}", request.PriceM2.Value);
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully patched price for product {ProductId} and stock {StockId}", productId, stockId);
                return Ok(new { message = "Price updated successfully", price });
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error patching price for product {ProductId} and stock {StockId}", productId, stockId);
                return StatusCode(500, new { message = "Error updating price in database" });
            }
        }

        /// <summary>
        /// DTO для частичного обновления цены
        /// </summary>
        public class PricePatchRequest
        {
            [JsonPropertyName("nds")]
            [Range(0, 100)]
            public decimal? NDS { get; set; }
            
            [JsonPropertyName("priceT")]
            [Range(0, double.MaxValue)]
            public decimal? PriceT { get; set; }
            
            [JsonPropertyName("priceLimitT1")]
            [Range(0, double.MaxValue)]
            public decimal? PriceLimitT1 { get; set; }
            
            [JsonPropertyName("priceT1")]
            [Range(0, double.MaxValue)]
            public decimal? PriceT1 { get; set; }
            
            [JsonPropertyName("priceLimitT2")]
            [Range(0, double.MaxValue)]
            public decimal? PriceLimitT2 { get; set; }
            
            [JsonPropertyName("priceT2")]
            [Range(0, double.MaxValue)]
            public decimal? PriceT2 { get; set; }
            
            [JsonPropertyName("priceM")]
            [Range(0, double.MaxValue)]
            public decimal? PriceM { get; set; }
            
            [JsonPropertyName("priceLimitM1")]
            [Range(0, double.MaxValue)]
            public decimal? PriceLimitM1 { get; set; }
            
            [JsonPropertyName("priceM1")]
            [Range(0, double.MaxValue)]
            public decimal? PriceM1 { get; set; }
            
            [JsonPropertyName("priceLimitM2")]
            [Range(0, double.MaxValue)]
            public decimal? PriceLimitM2 { get; set; }
            
            [JsonPropertyName("priceM2")]
            [Range(0, double.MaxValue)]
            public decimal? PriceM2 { get; set; }
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
            _logger.LogInformation("Creating new price for product {ProductId} and stock {StockId}", request.ProductId, request.StockId);
            
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for creating price - product {ProductId} and stock {StockId}", request.ProductId, request.StockId);
                return BadRequest(ModelState);
            }

            // Проверяем существует ли уже цена для этого продукта и склада
            var existingPrice = await _context.Prices
                .FirstOrDefaultAsync(p => p.ID == request.ProductId && p.IDStock == request.StockId);

            if (existingPrice != null)
            {
                _logger.LogWarning("Price already exists for product {ProductId} and stock {StockId}", request.ProductId, request.StockId);
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
                _logger.LogInformation("Successfully created price for product {ProductId} and stock {StockId}", request.ProductId, request.StockId);
                return CreatedAtAction(nameof(GetPrice), new { productId = price.ID, stockId = price.IDStock }, price);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error creating price for product {ProductId} and stock {StockId}", request.ProductId, request.StockId);
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
            public string StockId { get; set; } = string.Empty;
        }
    }
}