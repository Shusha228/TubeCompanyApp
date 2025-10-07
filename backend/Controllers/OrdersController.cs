using Microsoft.AspNetCore.Mvc;
using backend.Models;
using backend.Services;
using backend.Models.Entities;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IOrderService orderService, ILogger<OrdersController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder([FromBody] Models.Order.CreateOrderRequest request)
        {
            try
            {
                _logger.LogInformation($"Creating order for user {request.TelegramUserId}");
                
                if (request.Items == null || !request.Items.Any())
                {
                    return BadRequest(new { error = "Order must contain at least one item" });
                }

                if (string.IsNullOrEmpty(request.CustomerInfo.FirstName) || 
                    string.IsNullOrEmpty(request.CustomerInfo.LastName) ||
                    string.IsNullOrEmpty(request.CustomerInfo.Inn) ||
                    string.IsNullOrEmpty(request.CustomerInfo.Phone) ||
                    string.IsNullOrEmpty(request.CustomerInfo.Email))
                {
                    return BadRequest(new { error = "All customer information fields are required" });
                }
                
                var order = await _orderService.CreateOrderAsync(request);
                
                _logger.LogInformation($"Order created successfully: {order.Id}");
                
                return Ok(new { 
                    success = true, 
                    orderId = order.Id,
                    totalAmount = order.TotalAmount,
                    message = "Order created successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order");
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("user/{telegramUserId}")]
        public async Task<ActionResult<List<Order>>> GetUserOrders(long telegramUserId)
        {
            try
            {
                var orders = await _orderService.GetUserOrdersAsync(telegramUserId);
                return Ok(new {
                    success = true,
                    orders = orders,
                    count = orders.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting orders for user {telegramUserId}");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("{orderId}")]
        public async Task<ActionResult<Order>> GetOrder(string orderId)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(orderId);
                if (order == null)
                {
                    return NotFound(new { error = "Order not found" });
                }
                
                return Ok(new {
                    success = true,
                    order = order
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting order {orderId}");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok(new { 
                message = "Orders controller is working!",
                timestamp = DateTime.UtcNow
            });
        }
    }
}