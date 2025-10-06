using backend.Models.Entities;
using backend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace backend.Services
{
    public interface ICartService
    {
        Task<List<CartItem>> GetCartItemsAsync(int userId);
        Task<CartItem?> GetCartItemAsync(int userId, int productId);
        Task AddToCartAsync(int userId, int productId, decimal quantity, bool isInMeters = true);
        Task<bool> UpdateCartItemAsync(int userId, int productId, decimal quantity);
        Task<bool> RemoveFromCartAsync(int userId, int productId);
        Task ClearCartAsync(int userId);
        Task<CartSummaryResponse> GetCartSummaryAsync(int userId);
        Task<Order> MoveCartToOrderAsync(int userId, CustomerInfo customerInfo);
    }

    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _context;
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly ILogger<CartService> _logger;

        public CartService(
            ApplicationDbContext context,
            IProductService productService,
            IOrderService orderService,
            ILogger<CartService> logger)
        {
            _context = context;
            _productService = productService;
            _orderService = orderService;
            _logger = logger;
        }

        public async Task<List<CartItem>> GetCartItemsAsync(int userId)
        {
            return await _context.CartItems
                .Where(ci => ci.UserId == userId)
                .Include(ci => ci.Product)
                .ToListAsync();
        }

        public async Task<CartItem?> GetCartItemAsync(int userId, int productId)
        {
            return await _context.CartItems
                .Include(ci => ci.Product)
                .FirstOrDefaultAsync(ci => ci.UserId == userId && ci.ProductId == productId);
        }

        public async Task AddToCartAsync(int userId, int productId, decimal quantity, bool isInMeters = true)
        {
            var existingItem = await GetCartItemAsync(userId, productId);

            if (existingItem != null)
            {
                existingItem.Quantity = quantity;
                existingItem.IsInMeters = isInMeters;
                existingItem.UpdatedAt = DateTime.UtcNow;
                await UpdateCartItemPrices(existingItem);
                _logger.LogInformation("Updated existing cart item for user {UserId}, product {ProductId}", userId, productId);
            }
            else
            {
                var product = await _productService.GetProductByIdAsync(productId);
                if (product == null)
                {
                    throw new ArgumentException($"Product {productId} not found");
                }

                var priceRequest = new backend.Models.PriceCalculationRequest
                {
                    ProductId = productId,
                    Quantity = quantity,
                    IsInMeters = isInMeters
                };
                var priceResponse = await _productService.CalculatePriceAsync(priceRequest);

                // ИСПРАВЬ ЭТИ СТРОКИ - используй правильные свойства из backend.Models.PriceCalculationResponse
                var cartItem = new CartItem
                {
                    UserId = userId,
                    ProductId = productId,
                    ProductName = product.Name,
                    Quantity = quantity,
                    IsInMeters = isInMeters,
                    UnitPrice = priceResponse.FinalPrice / quantity, // РАССЧИТЫВАЕМ UnitPrice
                    FinalPrice = priceResponse.FinalPrice,
                    AddedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.CartItems.Add(cartItem);
                _logger.LogInformation("Added new cart item for user {UserId}, product {ProductId}", userId, productId);
            }

            await _context.SaveChangesAsync();
        }
        public async Task<bool> UpdateCartItemAsync(int userId, int productId, decimal quantity)
        {
            var cartItem = await GetCartItemAsync(userId, productId);
            if (cartItem == null) return false;

            cartItem.Quantity = quantity;
            cartItem.UpdatedAt = DateTime.UtcNow;
            await UpdateCartItemPrices(cartItem);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveFromCartAsync(int userId, int productId)
        {
            var cartItem = await GetCartItemAsync(userId, productId);
            if (cartItem == null) return false;

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task ClearCartAsync(int userId)
        {
            var cartItems = await GetCartItemsAsync(userId);
            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Cleared cart for user {UserId}, removed {Count} items", userId, cartItems.Count);
        }

        public async Task<CartSummaryResponse> GetCartSummaryAsync(int userId)
        {
            var cartItems = await GetCartItemsAsync(userId);
            return new CartSummaryResponse
            {
                UserId = userId,
                TotalItems = cartItems.Count,
                TotalAmount = cartItems.Sum(ci => ci.FinalPrice)
            };
        }

        public async Task<Order> MoveCartToOrderAsync(int userId, CustomerInfo customerInfo)
        {
            var cartItems = await GetCartItemsAsync(userId);
            if (!cartItems.Any()) throw new InvalidOperationException("Cart is empty");

            // Используем OrderCreateRequest из backend.Services
            var orderRequest = new OrderCreateRequest
            {
                TelegramUserId = userId,
                CustomerInfo = customerInfo,
                Items = cartItems.Select(ci => new OrderItemRequest
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    IsInMeters = ci.IsInMeters
                }).ToList()
            };

            var order = await _orderService.CreateOrderAsync(orderRequest);
            await ClearCartAsync(userId);
            return order;
        }

        private async Task UpdateCartItemPrices(CartItem cartItem)
        {
            // Используем backend.Models.PriceCalculationRequest
            var priceRequest = new backend.Models.PriceCalculationRequest
            {
                ProductId = cartItem.ProductId,
                Quantity = cartItem.Quantity,
                IsInMeters = cartItem.IsInMeters
            };
            var priceResponse = await _productService.CalculatePriceAsync(priceRequest);
            cartItem.UnitPrice = priceResponse.UnitPrice;
            cartItem.FinalPrice = priceResponse.FinalPrice;
        }
    }

    public class CartSummaryResponse
    {
        public int UserId { get; set; }
        public int TotalItems { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class OrderCreateRequest
    {
        public long TelegramUserId { get; set; }
        public CustomerInfo CustomerInfo { get; set; } = new CustomerInfo();
        public List<OrderItemRequest> Items { get; set; } = new List<OrderItemRequest>();
    }

    public class OrderItemRequest
    {
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
        public bool IsInMeters { get; set; } = true;
    }
}