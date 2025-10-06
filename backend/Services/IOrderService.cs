using backend.Models;

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
        private readonly List<Order> _orders = new();
        private readonly IProductService _productService;

        public OrderService(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<Order> CreateOrderAsync(CreateOrderRequest request)
        {
            foreach (var item in request.Items)
            {
                var priceResponse = await _productService.CalculatePriceAsync(new PriceCalculationRequest
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    IsInMeters = item.IsInMeters
                });
                
                item.FinalPrice = priceResponse.FinalPrice;
            }

            var totalAmount = request.Items.Sum(i => i.FinalPrice);

            var order = new Order
            {
                Id = Guid.NewGuid().ToString("N")[..8].ToUpper(),
                TelegramUserId = request.TelegramUserId,
                FirstName = request.CustomerInfo.FirstName,
                LastName = request.CustomerInfo.LastName,
                Inn = request.CustomerInfo.Inn,
                Phone = request.CustomerInfo.Phone,
                Email = request.CustomerInfo.Email,
                Items = request.Items,
                TotalAmount = totalAmount,
                CreatedAt = DateTime.UtcNow,
                Status = "Pending"
            };

            _orders.Add(order);
            return order;
        }

        public Task<Order?> GetOrderByIdAsync(string orderId)
        {
            var order = _orders.FirstOrDefault(o => o.Id == orderId);
            return Task.FromResult(order);
        }

        public Task<List<Order>> GetUserOrdersAsync(long telegramUserId)
        {
            var userOrders = _orders.Where(o => o.TelegramUserId == telegramUserId).ToList();
            return Task.FromResult(userOrders);
        }
    }
}