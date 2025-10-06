using backend.Models.Entities;
using backend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace backend.Services
{
    public interface ICartService
    {
        Task<CartItem> AddToCartAsync(int userId, int productId, string productName, decimal quantity, bool isInMeters, decimal unitPrice, decimal finalPrice);
        Task<bool> RemoveFromCartAsync(int userId, int productId);
        Task ClearCartAsync(int userId);
        Task<List<CartItem>> GetCartItemsAsync(int userId);
        Task<CartItem?> UpdateCartItemQuantityAsync(int userId, int productId, decimal newQuantity);
        Task<decimal> GetCartTotalAsync(int userId);
        Task<int> GetCartItemsCountAsync(int userId);
    }
    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CartService> _logger;

        public CartService(ApplicationDbContext context, ILogger<CartService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<CartItem> AddToCartAsync(int userId, int productId, string productName, decimal quantity, bool isInMeters, decimal unitPrice, decimal finalPrice)
        {
            try
            {
                var existingItem = await _context.CartItems
                    .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

                if (existingItem != null)
                {
                    // Обновляем существующий товар
                    existingItem.Quantity += quantity;
                    existingItem.FinalPrice = finalPrice;
                    existingItem.UnitPrice = unitPrice;
                    existingItem.UpdatedAt = DateTime.UtcNow;

                    _context.CartItems.Update(existingItem);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"Updated cart item for user {userId}, product {productId}, new quantity: {existingItem.Quantity}");
                    return existingItem;
                }
                else
                {
                    // Добавляем новый товар
                    var cartItem = new CartItem
                    {
                        UserId = userId,
                        ProductId = productId,
                        ProductName = productName,
                        Quantity = quantity,
                        IsInMeters = isInMeters,
                        UnitPrice = unitPrice,
                        FinalPrice = finalPrice,
                        AddedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    _context.CartItems.Add(cartItem);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"Added new cart item for user {userId}, product {productId}, quantity: {quantity}");
                    return cartItem;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding to cart for user {userId}, product {productId}");
                throw;
            }
        }

        public async Task<bool> RemoveFromCartAsync(int userId, int productId)
        {
            try
            {
                var cartItem = await _context.CartItems
                    .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

                if (cartItem == null)
                {
                    return false;
                }

                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Removed cart item for user {userId}, product {productId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error removing from cart for user {userId}, product {productId}");
                throw;
            }
        }

        public async Task ClearCartAsync(int userId)
        {
            try
            {
                var cartItems = await _context.CartItems
                    .Where(c => c.UserId == userId)
                    .ToListAsync();

                if (cartItems.Any())
                {
                    _context.CartItems.RemoveRange(cartItems);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"Cleared cart for user {userId}, removed {cartItems.Count} items");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error clearing cart for user {userId}");
                throw;
            }
        }

        public async Task<List<CartItem>> GetCartItemsAsync(int userId)
        {
            try
            {
                return await _context.CartItems
                    .Where(c => c.UserId == userId)
                    .OrderByDescending(c => c.AddedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting cart items for user {userId}");
                throw;
            }
        }

        public async Task<CartItem?> UpdateCartItemQuantityAsync(int userId, int productId, decimal newQuantity)
        {
            try
            {
                var cartItem = await _context.CartItems
                    .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

                if (cartItem == null)
                {
                    return null;
                }

                if (newQuantity <= 0)
                {
                    // Если количество <= 0, удаляем товар из корзины
                    await RemoveFromCartAsync(userId, productId);
                    return null;
                }

                // Пересчитываем цену на основе нового количества
                cartItem.Quantity = newQuantity;
                cartItem.FinalPrice = cartItem.UnitPrice * newQuantity;
                cartItem.UpdatedAt = DateTime.UtcNow;

                _context.CartItems.Update(cartItem);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Updated cart item quantity for user {userId}, product {productId}, new quantity: {newQuantity}");
                return cartItem;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating cart item quantity for user {userId}, product {productId}");
                throw;
            }
        }

        public async Task<decimal> GetCartTotalAsync(int userId)
        {
            try
            {
                return await _context.CartItems
                    .Where(c => c.UserId == userId)
                    .SumAsync(c => c.FinalPrice);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error calculating cart total for user {userId}");
                throw;
            }
        }

        public async Task<int> GetCartItemsCountAsync(int userId)
        {
            try
            {
                return await _context.CartItems
                    .Where(c => c.UserId == userId)
                    .CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting cart items count for user {userId}");
                throw;
            }
        }
    }
}