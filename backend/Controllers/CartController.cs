using Microsoft.AspNetCore.Mvc;
using backend.Services;
using backend.Models.Entities;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [SwaggerTag("Управление корзиной покупок")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly ILogger<CartController> _logger;

        public CartController(ICartService cartService, ILogger<CartController> logger)
        {
            _cartService = cartService;
            _logger = logger;
        }

        [HttpGet("{userId}/paged")]
        [SwaggerOperation(Summary = "Получить корзину с пагинацией")]
        [SwaggerResponse(200, "Корзина получена", typeof(CartPaginationResponse))]
        [SwaggerResponse(400, "Неверные параметры пагинации")]
        [SwaggerResponse(404, "Пользователь не найден")]
        [SwaggerResponse(500, "Ошибка сервера")]
        public async Task<ActionResult<CartPaginationResponse>> GetCartPaged(
            int userId,
            int from = 0,
            int to = 20)
        {
            try
            {
                var cartResponse = await _cartService.GetCartItemsPagedAsync(userId, from, to);
                return Ok(cartResponse);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, $"User not found for cart operation: {userId}");
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, $"Invalid pagination parameters for user {userId}");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting paged cart for user {userId}");
                return StatusCode(500, "Ошибка при получении корзины");
            }
        }

        [HttpPost("{userId}/items")]
        [SwaggerOperation(Summary = "Добавить товар в корзину")]
        [SwaggerResponse(200, "Товар добавлен в корзину", typeof(CartItem))]
        [SwaggerResponse(400, "Неверные данные запроса")]
        [SwaggerResponse(404, "Пользователь не найден")]
        [SwaggerResponse(500, "Ошибка сервера")]
        public async Task<ActionResult<CartItem>> AddToCart(
            int userId,
            [FromBody] AddToCartRequest request)
        {
            try
            {
                if (request.Quantity <= 0)
                {
                    return BadRequest("Количество должно быть больше 0");
                }

                var cartItem = await _cartService.AddToCartAsync(
                    userId,
                    request.StockId,
                    request.ProductId,
                    request.ProductName,
                    request.Quantity,
                    request.IsInMeters,
                    request.UnitPrice,
                    request.FinalPrice
                );

                return Ok(cartItem);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, $"Validation error adding to cart for user {userId}");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding to cart for user {userId}");
                return StatusCode(500, "Ошибка при добавлении товара в корзину");
            }
        }

        [HttpDelete("{userId}/items/{stockId}/{productId}")]
        [SwaggerOperation(Summary = "Удалить товар из корзины")]
        [SwaggerResponse(204, "Товар удален из корзины")]
        [SwaggerResponse(404, "Пользователь или товар не найден")]
        [SwaggerResponse(500, "Ошибка сервера")]
        public async Task<ActionResult> RemoveFromCart(int userId, string stockId, int productId)
        {
            try
            {
                var result = await _cartService.RemoveFromCartAsync(userId, stockId, productId);

                if (!result)
                {
                    return NotFound("Товар не найден в корзине");
                }

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, $"User not found for cart operation: {userId}");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    $"Error removing from cart for user {userId}, stock {stockId}, product {productId}");
                return StatusCode(500, "Ошибка при удалении товара из корзины");
            }
        }

        [HttpDelete("{userId}")]
        [SwaggerOperation(Summary = "Очистить корзину")]
        [SwaggerResponse(204, "Корзина очищена")]
        [SwaggerResponse(404, "Пользователь не найден")]
        [SwaggerResponse(500, "Ошибка сервера")]
        public async Task<ActionResult> ClearCart(int userId)
        {
            try
            {
                await _cartService.ClearCartAsync(userId);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, $"User not found for cart operation: {userId}");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error clearing cart for user {userId}");
                return StatusCode(500, "Ошибка при очистке корзины");
            }
        }

        [HttpPut("{userId}/items/{stockId}/{productId}")]
        [SwaggerOperation(Summary = "Обновить количество товара")]
        [SwaggerResponse(200, "Количество обновлено", typeof(CartItem))]
        [SwaggerResponse(400, "Неверное количество")]
        [SwaggerResponse(404, "Пользователь или товар не найден")]
        [SwaggerResponse(500, "Ошибка сервера")]
        public async Task<ActionResult<CartItem>> UpdateQuantity(
            int userId,
            string stockId,
            int productId,
            [FromBody] UpdateQuantityRequest request)
        {
            try
            {
                if (request.NewQuantity <= 0)
                {
                    return BadRequest("Количество должно быть больше 0");
                }

                var cartItem =
                    await _cartService.UpdateCartItemQuantityAsync(userId, stockId, productId, request.NewQuantity);

                if (cartItem == null)
                {
                    return NotFound("Товар не найден в корзине");
                }

                return Ok(cartItem);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, $"User not found for cart operation: {userId}");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    $"Error updating quantity for user {userId}, stock {stockId}, product {productId}");
                return StatusCode(500, "Ошибка при обновлении количества товара");
            }
        }

        [HttpGet("{userId}/total")]
        [SwaggerOperation(Summary = "Получить сумму корзины")]
        [SwaggerResponse(200, "Сумма корзины", typeof(decimal))]
        [SwaggerResponse(404, "Пользователь не найден")]
        [SwaggerResponse(500, "Ошибка сервера")]
        public async Task<ActionResult<decimal>> GetCartTotal(int userId)
        {
            try
            {
                var total = await _cartService.GetCartTotalAsync(userId);
                return Ok(total);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, $"User not found for cart operation: {userId}");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting cart total for user {userId}");
                return StatusCode(500, "Ошибка при получении суммы корзины");
            }
        }

        [HttpGet("{userId}/count")]
        [SwaggerOperation(Summary = "Получить количество товаров")]
        [SwaggerResponse(200, "Количество товаров", typeof(int))]
        [SwaggerResponse(404, "Пользователь не найден")]
        [SwaggerResponse(500, "Ошибка сервера")]
        public async Task<ActionResult<int>> GetCartItemsCount(int userId)
        {
            try
            {
                var count = await _cartService.GetCartItemsCountAsync(userId);
                return Ok(count);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, $"User not found for cart operation: {userId}");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting cart items count for user {userId}");
                return StatusCode(500, "Ошибка при получении количества товаров в корзине");
            }
        }

        [HttpGet("{userId}/validate")]
        [SwaggerOperation(Summary = "Проверить существование пользователя")]
        [SwaggerResponse(200, "Пользователь существует", typeof(bool))]
        [SwaggerResponse(500, "Ошибка сервера")]
        public async Task<ActionResult<bool>> ValidateUser(int userId)
        {
            try
            {
                var exists = await _cartService.ValidateUserExistsAsync(userId);
                return Ok(exists);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error validating user existence for user {userId}");
                return StatusCode(500, "Ошибка при проверке пользователя");
            }
        }

        [HttpGet("{userId}/search")]
        [SwaggerOperation(Summary = "Поиск товаров в корзине")]
        [SwaggerResponse(200, "Результаты поиска", typeof(List<CartItem>))]
        [SwaggerResponse(400, "Неверный поисковый запрос")]
        [SwaggerResponse(404, "Пользователь не найден")]
        [SwaggerResponse(500, "Ошибка сервера")]
        public async Task<ActionResult<List<CartItem>>> SearchInCart(
            int userId,
            [FromQuery] string term)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(term))
                {
                    return BadRequest("Поисковый запрос не может быть пустым");
                }

                var searchResults = await _cartService.SearchInCartAsync(userId, term);
                return Ok(new
                {
                    success = true,
                    data = searchResults,
                    count = searchResults.Count,
                    searchTerm = term
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, $"User not found for cart search: {userId}");
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error searching in cart for user {userId} with term: {term}");
                return StatusCode(500, new { error = "Ошибка при поиске в корзине" });
            }
        }

        [HttpGet("{userId}/search/paged")]
        [SwaggerOperation(Summary = "Поиск товаров в корзине с пагинацией")]
        [SwaggerResponse(200, "Результаты поиска", typeof(CartPaginationResponse))]
        [SwaggerResponse(400, "Неверные параметры")]
        [SwaggerResponse(404, "Пользователь не найден")]
        [SwaggerResponse(500, "Ошибка сервера")]
        public async Task<ActionResult<CartPaginationResponse>> SearchInCartPaged(
            int userId,
            [FromQuery] string term,
            [FromQuery] int from = 0,
            [FromQuery] int to = 20)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(term))
                {
                    return BadRequest(new { error = "Поисковый запрос не может быть пустым" });
                }

                var searchResults = await _cartService.SearchInCartPagedAsync(userId, term, from, to);
                return Ok(new
                {
                    success = true,
                    data = searchResults
                });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, $"Invalid pagination parameters for user {userId}");
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, $"User not found for cart search: {userId}");
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error searching in cart paged for user {userId} with term: {term}");
                return StatusCode(500, new { error = "Ошибка при поиске в корзине" });
            }
        }
    }

    public class AddToCartRequest
    {
        public string StockId { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public bool IsInMeters { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal FinalPrice { get; set; }
    }

    public class UpdateQuantityRequest
    {
        public decimal NewQuantity { get; set; }
    }

    public class CartPaginationResponse
    {
        public List<CartItem> Items { get; set; } = new List<CartItem>();
        public PaginationMeta Meta { get; set; } = new PaginationMeta();
    }

    public class PaginationMeta
    {
        public int TotalPages { get; set; }
        public int Page { get; set; }
        public int PageLimit { get; set; }
        public int TotalCount { get; set; }
        public string? SearchTerm { get; set; }
    }
}