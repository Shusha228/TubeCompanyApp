using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace backend.Services
{
    public interface ITelegramService
    {
        Task HandleUpdateAsync(Update update);
    }

    public class TelegramService : ITelegramService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<TelegramService> _logger;

        public TelegramService(IConfiguration configuration, ILogger<TelegramService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task HandleUpdateAsync(Update update)
        {
            var botToken = _configuration["Telegram:BotToken"];
            if (string.IsNullOrEmpty(botToken))
            {
                _logger.LogError("Bot token is not configured");
                return;
            }

            var botClient = new TelegramBotClient(botToken);

            try
            {
                // Обрабатываем сообщения
                if (update.Message != null)
                {
                    _logger.LogInformation($"Received message from {update.Message.From?.FirstName}: {update.Message.Text}");

                    if (update.Message.Text == "/start")
                    {
                        var baseUrl = _configuration["App:BaseUrl"];
                        var webAppUrl = $"{baseUrl}/miniapp";
                        
                        _logger.LogInformation($"Sending start message with webapp URL: {webAppUrl}");

                        var keyboard = new InlineKeyboardMarkup(new[]
                        {
                            new[]
                            {
                                InlineKeyboardButton.WithWebApp("🏭 Открыть каталог труб", webAppUrl)
                            }
                        });

                        await botClient.SendMessage(
                            chatId: update.Message.Chat.Id,
                            text: "🏭 Добро пожаловать в каталог трубной продукции!\n\n" +
                                  "Нажмите кнопку ниже чтобы открыть каталог:",
                            replyMarkup: keyboard);

                        _logger.LogInformation("Start message sent successfully");
                    }
                    else
                    {
                        // Ответ на любое другое сообщение
                        await botClient.SendMessage(
                            chatId: update.Message.Chat.Id,
                            text: "Для начала работы отправьте /start");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling Telegram update");
            }
        }
    }
}