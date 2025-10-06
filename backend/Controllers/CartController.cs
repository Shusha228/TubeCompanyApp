using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using backend.Models.Entities;
using backend.Services;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [SwaggerTag("Управление корзиной пользователя")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly ILogger<CartController> _logger;

        public CartController(ICartService cartService, ILogger<CartController> logger)
        {
            _cartService = cartService;
            _logger = logger;
        }

        /// <summary>
        /// DTO для добавления товара в корзину
        /// </summary>
        public class AddToCartRequest
        {
            [SwaggerParameter("ID пользователя", Required = true)]
            [Required]
            public int UserId { get; set; }

            [SwaggerParameter("ID товара", Required = true)]
            [Required]
            public int ProductId { get; set; }

            [SwaggerParameter("Количество", Required = true)]
            [Required]
            [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
            public decimal Quantity { get; set; }

            [SwaggerParameter("В метрах (true) или тоннах (false)")]
            public bool IsInMeters { get; set; } = true;
        }

        /// <summary>
        /// DTO для обновления количества товара в корзине
        /// </summary>
        public class UpdateCartItemRequest
        {
            [SwaggerParameter("Новое количество", Required = true)]
            [Required]
            [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
            public decimal Quantity { get; set; }
        }

        [HttpGet("{userId}")]
        [SwaggerOperation(
            Summary = "Получить содержимое корзины пользователя",
            Description = "Возвращает все товары в корзине указанного пользователя"
        )]
        [SwaggerResponse(200, "Корзина пользователя", typeof(List<CartItem>))]
        public async Task<ActionResult<List<CartItem>>> GetCart(int userId)
        {
            _logger.LogInformation("Getting cart for user {UserId}", userId);

            var cartItems = await _cartService.GetCartItemsAsync(userId);

            _logger.LogInformation("Found {Count} items in cart for user {UserId}", cartItems.Count, userId);
            return Ok(cartItems);
        }

        [HttpPost("add")]
        [SwaggerOperation(
            Summary = "Добавить товар в корзину",
            Description = "Добавляет товар в корзину пользователя или обновляет количество если товар уже есть"
        )]
        [SwaggerResponse(200, "Товар добавлен в корзину")]
        [SwaggerResponse(400, "Неверные данные запроса")]
        public async Task<ActionResult> AddToCart([FromBody] AddToCartRequest request)
        {
            _logger.LogInformation("Adding product {ProductId} to cart for user {UserId}", request.ProductId, request.UserId);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for adding to cart");
                return BadRequest(ModelState);
            }

            try
            {
                await _cartService.AddToCartAsync(request.UserId, request.ProductId, request.Quantity, request.IsInMeters);
                
                _logger.LogInformation("Product {ProductId} added to cart for user {UserId}", request.ProductId, request.UserId);
                return Ok(new { message = "Product added to cart successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding product {ProductId} to cart for user {UserId}", request.ProductId, request.UserId);
                return StatusCode(500, new { message = "Error adding product to cart", error = ex.Message });
            }
        }

        [HttpPut("{userId}/items/{productId}")]
        [SwaggerOperation(
            Summary = "Обновить количество товара в корзине",
            Description = "Обновляет количество указанного товара в корзине пользователя"
        )]
        [SwaggerResponse(200, "Количество товара обновлено")]
        [SwaggerResponse(404, "Товар не найден в корзине")]
        public async Task<ActionResult> UpdateCartItem(int userId, int productId, [FromBody] UpdateCartItemRequest request)
        {
            _logger.LogInformation("Updating product {ProductId} quantity to {Quantity} for user {UserId}", 
                productId, request.Quantity, userId);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _cartService.UpdateCartItemAsync(userId, productId, request.Quantity);

                if (!result)
                {
                    _logger.LogWarning("Product {ProductId} not found in cart for user {UserId}", productId, userId);
                    return NotFound(new { message = $"Product {productId} not found in cart" });
                }

                _logger.LogInformation("Product {ProductId} quantity updated to {Quantity} for user {UserId}", 
                    productId, request.Quantity, userId);
                return Ok(new { message = "Cart item updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating cart item for user {UserId}, product {ProductId}", userId, productId);
                return StatusCode(500, new { message = "Error updating cart item" });
            }
        }

        [HttpDelete("{userId}/items/{productId}")]
        [SwaggerOperation(
            Summary = "Удалить товар из корзины",
            Description = "Удаляет указанный товар из корзины пользователя"
        )]
        [SwaggerResponse(200, "Товар удален из корзины")]
        [SwaggerResponse(404, "Товар не найден в корзине")]
        public async Task<ActionResult> RemoveFromCart(int userId, int productId)
        {
            _logger.LogInformation("Removing product {ProductId} from cart for user {UserId}", productId, userId);

            try
            {
                var result = await _cartService.RemoveFromCartAsync(userId, productId);

                if (!result)
                {
                    _logger.LogWarning("Product {ProductId} not found in cart for user {UserId}", productId, userId);
                    return NotFound(new { message = $"Product {productId} not found in cart" });
                }

                _logger.LogInformation("Product {ProductId} removed from cart for user {UserId}", productId, userId);
                return Ok(new { message = "Product removed from cart successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing product {ProductId} from cart for user {UserId}", productId, userId);
                return StatusCode(500, new { message = "Error removing product from cart" });
            }
        }

        [HttpDelete("{userId}")]
        [SwaggerOperation(
            Summary = "Очистить корзину пользователя",
            Description = "Удаляет все товары из корзины указанного пользователя"
        )]
        [SwaggerResponse(200, "Корзина очищена")]
        public async Task<ActionResult> ClearCart(int userId)
        {
            _logger.LogInformation("Clearing cart for user {UserId}", userId);

            try
            {
                await _cartService.ClearCartAsync(userId);
                
                _logger.LogInformation("Cart cleared for user {UserId}", userId);
                return Ok(new { message = "Cart cleared successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing cart for user {UserId}", userId);
                return StatusCode(500, new { message = "Error clearing cart" });
            }
        }

        [HttpGet("{userId}/summary")]
        [SwaggerOperation(
            Summary = "Получить сводку по корзине",
            Description = "Возвращает общую информацию о корзине: количество товаров, общая сумма и т.д."
        )]
        [SwaggerResponse(200, "Сводка по корзине", typeof(CartSummaryResponse))]
        public async Task<ActionResult<CartSummaryResponse>> GetCartSummary(int userId)
        {
            _logger.LogInformation("Getting cart summary for user {UserId}", userId);

            var summary = await _cartService.GetCartSummaryAsync(userId);

            _logger.LogInformation("Cart summary for user {UserId}: {TotalItems} items, {TotalAmount} total", 
                userId, summary.TotalItems, summary.TotalAmount);
            return Ok(summary);
        }

        [HttpPost("{userId}/move-to-order")]
        [SwaggerOperation(
            Summary = "Переместить корзину в заказ",
            Description = "Создает заказ из текущей корзины и очищает корзину"
        )]
        [SwaggerResponse(200, "Заказ создан из корзины", typeof(Order))]
        [SwaggerResponse(400, "Корзина пуста")]
        public async Task<ActionResult<Order>> MoveCartToOrder(int userId, [FromBody] CustomerInfo customerInfo)
        {
            _logger.LogInformation("Moving cart to order for user {UserId}", userId);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var order = await _cartService.MoveCartToOrderAsync(userId, customerInfo);
                
                _logger.LogInformation("Order {OrderId} created from cart for user {UserId}", order.Id, userId);
                return Ok(order);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("empty"))
            {
                _logger.LogWarning("Cannot create order from empty cart for user {UserId}", userId);
                return BadRequest(new { message = "Cart is empty" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error moving cart to order for user {UserId}", userId);
                return StatusCode(500, new { message = "Error creating order from cart", error = ex.Message });
            }
        }
    }

    /// <summary>
    /// DTO для сводки по корзине
    /// </summary>
    public class CartSummaryResponse
    {
        public int UserId { get; set; }
        public int TotalItems { get; set; }
        public decimal TotalAmount { get; set; }
    }
}