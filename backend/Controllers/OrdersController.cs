using Microsoft.AspNetCore.Mvc;
using backend.Models;
using backend.Services;

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
        public async Task<ActionResult<Order>> CreateOrder([FromBody] CreateOrderRequest request)
        { 
            try
            {
                _logger.LogInformation($"Creating order for user {request.TelegramUserId}");
                
                var order = await _orderService.CreateOrderAsync(request);
                
                _logger.LogInformation($"Order created: {order.Id}");
                
                return Ok(order);
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
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("{orderId}")]
        public async Task<ActionResult<Order>> GetOrder(string orderId)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(orderId);
                if (order == null) return NotFound();
                return Ok(order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}