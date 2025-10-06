using backend.Models.Entities;
using backend.Models;
using backend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace backend.Services
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(CreateOrderRequest request);
        Task<Order?> GetOrderByIdAsync(string orderId);
        Task<List<Order>> GetUserOrdersAsync(long telegramUserId);
    }

    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly IProductService _productService;
        private readonly ITelegramNotificationService _telegramNotificationService;
        private readonly ILogger<OrderService> _logger;

        public OrderService(
            ApplicationDbContext context,
            IProductService productService,
            ITelegramNotificationService telegramNotificationService,
            ILogger<OrderService> logger)
        {
            _context = context;
            _productService = productService;
            _telegramNotificationService = telegramNotificationService;
            _logger = logger;
        }

        public async Task<Order> CreateOrderAsync(CreateOrderRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                var orderItems = new List<CartItem>();
                
                foreach (var requestItem in request.Items)
                {
                    var priceResponse = await _productService.CalculatePriceAsync(new PriceCalculationRequest
                    {
                        ProductId = requestItem.ProductId,
                        Quantity = requestItem.Quantity,
                        IsInMeters = requestItem.IsInMeters
                    });
                    
                    var product = await _productService.GetProductByIdAsync(requestItem.ProductId);
                    
                    var cartItem = new CartItem
                    {
                        ProductId = requestItem.ProductId,
                        ProductName = product?.Name ?? "Неизвестный товар",
                        Quantity = requestItem.Quantity,
                        IsInMeters = requestItem.IsInMeters,
                        FinalPrice = priceResponse.FinalPrice,
                        UnitPrice = priceResponse.FinalPrice / requestItem.Quantity
                    };

                    orderItems.Add(cartItem);
                }

                var totalAmount = orderItems.Sum(i => i.FinalPrice);

                var order = new Order
                {
                    Id = Guid.NewGuid().ToString("N")[..8].ToUpper(),
                    TelegramUserId = request.TelegramUserId,
                    FirstName = request.CustomerInfo.FirstName,
                    LastName = request.CustomerInfo.LastName,
                    Inn = request.CustomerInfo.Inn,
                    Phone = request.CustomerInfo.Phone,
                    Email = request.CustomerInfo.Email,
                    Items = orderItems,
                    TotalAmount = totalAmount,
                    CreatedAt = DateTime.UtcNow,
                    Status = "Pending",
                    AdminNotified = false
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                
                await _telegramNotificationService.NotifyAdminsAboutNewOrderAsync(order);

                await transaction.CommitAsync();
                
                _logger.LogInformation($"Order created successfully: {order.Id}");
                return order;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error creating order");
                throw;
            }
        }

        public async Task<Order?> GetOrderByIdAsync(string orderId)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task<List<Order>> GetUserOrdersAsync(long telegramUserId)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .Where(o => o.TelegramUserId == telegramUserId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }
    }
}