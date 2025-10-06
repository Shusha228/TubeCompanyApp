using Microsoft.AspNetCore.Mvc;
using backend.Services;
using Telegram.Bot.Types;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TelegramController : ControllerBase
    {
        private readonly ITelegramService _telegramService;
        private readonly ILogger<TelegramController> _logger;

        public TelegramController(ITelegramService telegramService, ILogger<TelegramController> logger)
        {
            _telegramService = telegramService;
            _logger = logger;
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> HandleWebhook([FromBody] Update update)
        {
            try
            {
                _logger.LogInformation("Received Telegram webhook");
                
                if (update == null)
                {
                    _logger.LogWarning("Empty update received");
                    return Ok();
                }

                _logger.LogInformation($"Update ID: {update.Id}, Type: {update.Type}");
                
                await _telegramService.HandleUpdateAsync(update);
                
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Telegram webhook");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok(new { message = "Telegram controller is working!" });
        }
    }
}