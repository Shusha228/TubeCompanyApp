using Microsoft.AspNetCore.Mvc;
using backend.Services;
using backend.Models.Entities;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;

namespace backend.Controllers
{
    /// <summary>
    /// API для управления корзиной покупок
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [SwaggerTag("Управление корзиной покупок - добавление, удаление, обновление товаров")]
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
        /// Получить содержимое корзины пользователя
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <returns>Список товаров в корзине</returns>
        [HttpGet("{userId}")]
        [SwaggerOperation(Summary = "Получить корзину пользователя", Description = "Возвращает все товары в корзине указанного пользователя")]
        [SwaggerResponse(200, "Корзина получена", typeof(List<CartItem>))]
        [SwaggerResponse(500, "Ошибка сервера")]
        public async Task<ActionResult<List<CartItem>>> GetCart(
            [SwaggerParameter("ID пользователя", Required = true)] int userId)
        {
            try
            {
                var cartItems = await _cartService.GetCartItemsAsync(userId);
                return Ok(cartItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting cart for user {userId}");
                return StatusCode(500, "Ошибка при получении корзины");
            }
        }

        /// <summary>
        /// Добавить товар в корзину
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <param name="request">Данные товара для добавления</param>
        /// <returns>Добавленный товар в корзине</returns>
        [HttpPost("{userId}/items")]
        [SwaggerOperation(Summary = "Добавить товар в корзину", Description = "Добавляет новый товар в корзину пользователя или обновляет количество существующего")]
        [SwaggerResponse(200, "Товар добавлен в корзину", typeof(CartItem))]
        [SwaggerResponse(400, "Неверные данные запроса")]
        [SwaggerResponse(500, "Ошибка сервера")]
        public async Task<ActionResult<CartItem>> AddToCart(
            [SwaggerParameter("ID пользователя", Required = true)] int userId,
            [SwaggerParameter("Данные товара для добавления", Required = true)]
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
                    request.ProductId, 
                    request.ProductName, 
                    request.Quantity, 
                    request.IsInMeters, 
                    request.UnitPrice, 
                    request.FinalPrice
                );

                return Ok(cartItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding to cart for user {userId}");
                return StatusCode(500, "Ошибка при добавлении товара в корзину");
            }
        }

        /// <summary>
        /// Удалить товар из корзины
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <param name="productId">ID товара</param>
        /// <returns>Результат операции</returns>
        [HttpDelete("{userId}/items/{productId}")]
        [SwaggerOperation(Summary = "Удалить товар из корзины", Description = "Удаляет конкретный товар из корзины пользователя")]
        [SwaggerResponse(204, "Товар удален из корзины")]
        [SwaggerResponse(404, "Товар не найден в корзине")]
        [SwaggerResponse(500, "Ошибка сервера")]
        public async Task<ActionResult> RemoveFromCart(
            [SwaggerParameter("ID пользователя", Required = true)] int userId,
            [SwaggerParameter("ID товара", Required = true)] int productId)
        {
            try
            {
                var result = await _cartService.RemoveFromCartAsync(userId, productId);
                
                if (!result)
                {
                    return NotFound("Товар не найден в корзине");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error removing from cart for user {userId}, product {productId}");
                return StatusCode(500, "Ошибка при удалении товара из корзины");
            }
        }

        /// <summary>
        /// Очистить корзину пользователя
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <returns>Результат операции</returns>
        [HttpDelete("{userId}")]
        [SwaggerOperation(Summary = "Очистить корзину", Description = "Полностью очищает корзину пользователя")]
        [SwaggerResponse(204, "Корзина очищена")]
        [SwaggerResponse(500, "Ошибка сервера")]
        public async Task<ActionResult> ClearCart(
            [SwaggerParameter("ID пользователя", Required = true)] int userId)
        {
            try
            {
                await _cartService.ClearCartAsync(userId);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error clearing cart for user {userId}");
                return StatusCode(500, "Ошибка при очистке корзины");
            }
        }

        /// <summary>
        /// Обновить количество товара в корзине
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <param name="productId">ID товара</param>
        /// <param name="request">Новое количество товара</param>
        /// <returns>Обновленный товар в корзине</returns>
        [HttpPut("{userId}/items/{productId}")]
        [SwaggerOperation(Summary = "Обновить количество товара", Description = "Изменяет количество конкретного товара в корзине пользователя")]
        [SwaggerResponse(200, "Количество обновлено", typeof(CartItem))]
        [SwaggerResponse(400, "Неверное количество")]
        [SwaggerResponse(404, "Товар не найден в корзине")]
        [SwaggerResponse(500, "Ошибка сервера")]
        public async Task<ActionResult<CartItem>> UpdateQuantity(
            [SwaggerParameter("ID пользователя", Required = true)] int userId,
            [SwaggerParameter("ID товара", Required = true)] int productId,
            [SwaggerParameter("Новое количество товара", Required = true)]
            [FromBody] UpdateQuantityRequest request)
        {
            try
            {
                if (request.NewQuantity <= 0)
                {
                    return BadRequest("Количество должно быть больше 0");
                }

                var cartItem = await _cartService.UpdateCartItemQuantityAsync(userId, productId, request.NewQuantity);
                
                if (cartItem == null)
                {
                    return NotFound("Товар не найден в корзине");
                }

                return Ok(cartItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating quantity for user {userId}, product {productId}");
                return StatusCode(500, "Ошибка при обновлении количества товара");
            }
        }

        /// <summary>
        /// Получить общую сумму корзины
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <returns>Общая сумма корзины</returns>
        [HttpGet("{userId}/total")]
        [SwaggerOperation(Summary = "Получить сумму корзины", Description = "Рассчитывает и возвращает общую стоимость всех товаров в корзине")]
        [SwaggerResponse(200, "Сумма корзины", typeof(decimal))]
        [SwaggerResponse(500, "Ошибка сервера")]
        public async Task<ActionResult<decimal>> GetCartTotal(
            [SwaggerParameter("ID пользователя", Required = true)] int userId)
        {
            try
            {
                var total = await _cartService.GetCartTotalAsync(userId);
                return Ok(total);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting cart total for user {userId}");
                return StatusCode(500, "Ошибка при получении суммы корзины");
            }
        }

        /// <summary>
        /// Получить количество товаров в корзине
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <returns>Количество товаров в корзине</returns>
        [HttpGet("{userId}/count")]
        [SwaggerOperation(Summary = "Получить количество товаров", Description = "Возвращает общее количество позиций в корзине пользователя")]
        [SwaggerResponse(200, "Количество товаров", typeof(int))]
        [SwaggerResponse(500, "Ошибка сервера")]
        public async Task<ActionResult<int>> GetCartItemsCount(
            [SwaggerParameter("ID пользователя", Required = true)] int userId)
        {
            try
            {
                var count = await _cartService.GetCartItemsCountAsync(userId);
                return Ok(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting cart items count for user {userId}");
                return StatusCode(500, "Ошибка при получении количества товаров в корзине");
            }
        }
    }

    /// <summary>
    /// Запрос на добавление товара в корзину
    /// </summary>
    public class AddToCartRequest
    {
        /// <summary>
        /// ID товара
        /// </summary>
        public int ProductId { get; set; }
        
        /// <summary>
        /// Название товара
        /// </summary>
        public string ProductName { get; set; } = string.Empty;
        
        /// <summary>
        /// Количество товара
        /// </summary>
        public decimal Quantity { get; set; }
        
        /// <summary>
        /// Флаг измерения в метрах (true - метры, false - тонны)
        /// </summary>
        public bool IsInMeters { get; set; }
        
        /// <summary>
        /// Цена за единицу
        /// </summary>
        public decimal UnitPrice { get; set; }
        
        /// <summary>
        /// Итоговая цена
        /// </summary>
        public decimal FinalPrice { get; set; }
    }

    /// <summary>
    /// Запрос на обновление количества товара
    /// </summary>
    public class UpdateQuantityRequest
    {
        /// <summary>
        /// Новое количество товара
        /// </summary>
        public decimal NewQuantity { get; set; }
    }
}