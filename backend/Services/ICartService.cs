using backend.Models.Entities;
using backend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace backend.Services
{
    public interface ICartService
    {
        Task<bool> ValidateUserExistsAsync(int userId);
        Task<CartItem> AddToCartAsync(int userId, string stockId, int productId, string productName, decimal quantity, bool isInMeters, decimal unitPrice, decimal finalPrice);
        Task<bool> RemoveFromCartAsync(int userId, string stockId, int productId);
        Task ClearCartAsync(int userId);
        Task<List<CartItem>> GetCartItemsAsync(int userId);
        Task<CartPaginationResponse> GetCartItemsPagedAsync(int userId, int from, int to);
        Task<CartItem?> UpdateCartItemQuantityAsync(int userId, string stockId, int productId, decimal newQuantity);
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

        public async Task<bool> ValidateUserExistsAsync(int userId)
        {
            try
            {
                return await _context.CustomerInfos
                    .AnyAsync(c => c.UserId == userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error validating user existence for user {userId}");
                throw;
            }
        }

        public async Task<CartItem> AddToCartAsync(int userId, string stockId, int productId, string productName, decimal quantity, bool isInMeters, decimal unitPrice, decimal finalPrice)
        {
            try
            {
                // Проверка существования пользователя
                // if (!await ValidateUserExistsAsync(userId))
                // {
                //     throw new InvalidOperationException($"Пользователь с ID '{userId}' не найден в системе. Пожалуйста, заполните информацию о себе перед добавлением товаров в корзину.");
                // }

                var stockExists = await _context.Stocks
                    .AnyAsync(s => s.IDStock == stockId);
                
                if (!stockExists)
                {
                    throw new InvalidOperationException($"Склад с ID '{stockId}' не найден");
                }

                var productExists = await _context.Nomenclatures
                    .AnyAsync(p => p.ID == productId);
                
                if (!productExists)
                {
                    throw new InvalidOperationException($"Товар с ID '{productId}' не найден");
                }

                var productInStock = await _context.Prices
                    .AnyAsync(ps => ps.IDStock == stockId && ps.ID == productId);
                
                if (!productInStock)
                {
                    throw new InvalidOperationException($"Товар с ID '{productId}' недоступен на складе '{stockId}'");
                }

                decimal calculatedFinalPrice = unitPrice * quantity;

                var existingItem = await _context.CartItems
                    .FirstOrDefaultAsync(c => c.UserId == userId && c.StockId == stockId && c.ProductId == productId && c.IsInMeters == isInMeters);

                if (existingItem != null)
                {
                    existingItem.Quantity += quantity;
                    existingItem.FinalPrice = existingItem.UnitPrice * existingItem.Quantity;
                    existingItem.UpdatedAt = DateTime.UtcNow;

                    _context.CartItems.Update(existingItem);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"Updated cart item for user {userId}, stock {stockId}, product {productId}, isInMeters: {isInMeters}, new quantity: {existingItem.Quantity}");
                    return existingItem;
                }
                else
                {
                    var cartItem = new CartItem
                    {
                        UserId = userId,
                        StockId = stockId,
                        ProductId = productId,
                        ProductName = productName,
                        Quantity = quantity,
                        IsInMeters = isInMeters,
                        UnitPrice = unitPrice,
                        FinalPrice = calculatedFinalPrice,
                        AddedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    _context.CartItems.Add(cartItem);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"Added new cart item for user {userId}, stock {stockId}, product {productId}, isInMeters: {isInMeters}, quantity: {quantity}");
                    return cartItem;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding to cart for user {userId}, stock {stockId}, product {productId}");
                throw;
            }
        }

        public async Task<bool> RemoveFromCartAsync(int userId, string stockId, int productId)
        {
            try
            {
                // Проверка существования пользователя
                if (!await ValidateUserExistsAsync(userId))
                {
                    throw new InvalidOperationException($"Пользователь с ID '{userId}' не найден в системе.");
                }

                var cartItem = await _context.CartItems
                    .FirstOrDefaultAsync(c => c.UserId == userId && c.StockId == stockId && c.ProductId == productId);

                if (cartItem == null)
                {
                    return false;
                }

                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Removed cart item for user {userId}, stock {stockId}, product {productId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error removing from cart for user {userId}, stock {stockId}, product {productId}");
                throw;
            }
        }

        public async Task ClearCartAsync(int userId)
        {
            try
            {
                // Проверка существования пользователя
                if (!await ValidateUserExistsAsync(userId))
                {
                    throw new InvalidOperationException($"Пользователь с ID '{userId}' не найден в системе.");
                }

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
                // Проверка существования пользователя
                if (!await ValidateUserExistsAsync(userId))
                {
                    throw new InvalidOperationException($"Пользователь с ID '{userId}' не найден в системе.");
                }

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

        public async Task<CartPaginationResponse> GetCartItemsPagedAsync(int userId, int from, int to)
        {
            try
            {
                // Проверка существования пользователя
                if (!await ValidateUserExistsAsync(userId))
                {
                    throw new InvalidOperationException($"Пользователь с ID '{userId}' не найден в системе.");
                }

                if (from < 0) throw new ArgumentException("From cannot be negative");
                if (to <= from) throw new ArgumentException("To must be greater than from");
                if (to - from > 100) throw new ArgumentException("Page size cannot exceed 100 items");
                
                var totalCount = await _context.CartItems
                    .Where(c => c.UserId == userId)
                    .CountAsync();

                var items = await _context.CartItems
                    .Where(c => c.UserId == userId)
                    .OrderByDescending(c => c.AddedAt)
                    .Skip(from)
                    .Take(to - from)
                    .ToListAsync();

                var pageSize = to - from;
                var currentPage = from / pageSize;
                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                return new CartPaginationResponse
                {
                    Items = items,
                    Meta = new PaginationMeta
                    {
                        TotalPages = totalPages,
                        Page = currentPage,
                        PageLimit = pageSize,
                        TotalCount = totalCount
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting paged cart items for user {userId}, from: {from}, to: {to}");
                throw;
            }
        }

        public async Task<CartItem?> UpdateCartItemQuantityAsync(int userId, string stockId, int productId, decimal newQuantity)
        {
            try
            {
                // Проверка существования пользователя
                if (!await ValidateUserExistsAsync(userId))
                {
                    throw new InvalidOperationException($"Пользователь с ID '{userId}' не найден в системе.");
                }

                var cartItem = await _context.CartItems
                    .FirstOrDefaultAsync(c => c.UserId == userId && c.StockId == stockId && c.ProductId == productId);

                if (cartItem == null)
                {
                    return null;
                }

                if (newQuantity <= 0)
                {
                    await RemoveFromCartAsync(userId, stockId, productId);
                    return null;
                }

                cartItem.Quantity = newQuantity;
                cartItem.FinalPrice = cartItem.UnitPrice * newQuantity;
                cartItem.UpdatedAt = DateTime.UtcNow;

                _context.CartItems.Update(cartItem);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Updated cart item quantity for user {userId}, stock {stockId}, product {productId}, new quantity: {newQuantity}");
                return cartItem;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating cart item quantity for user {userId}, stock {stockId}, product {productId}");
                throw;
            }
        }

        public async Task<decimal> GetCartTotalAsync(int userId)
        {
            try
            {
                // Проверка существования пользователя
                if (!await ValidateUserExistsAsync(userId))
                {
                    throw new InvalidOperationException($"Пользователь с ID '{userId}' не найден в системе.");
                }

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
                // Проверка существования пользователя
                if (!await ValidateUserExistsAsync(userId))
                {
                    throw new InvalidOperationException($"Пользователь с ID '{userId}' не найден в системе.");
                }

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
    }
}