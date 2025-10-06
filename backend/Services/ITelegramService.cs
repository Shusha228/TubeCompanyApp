using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
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
        private readonly ITelegramBotClient _botClient;

        public TelegramService(
            IConfiguration configuration, 
            ILogger<TelegramService> logger,
            ITelegramBotClient botClient)
        {
            _configuration = configuration;
            _logger = logger;
            _botClient = botClient;
        }

        public async Task HandleUpdateAsync(Update update)
        {
            try
            {
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

                        await _botClient.SendMessage(
                            chatId: update.Message.Chat.Id,
                            text: "🏭 Добро пожаловать в каталог трубной продукции!\n\n" +
                                  "Нажмите кнопку ниже чтобы открыть каталог:",
                            replyMarkup: keyboard);

                        _logger.LogInformation("Start message sent successfully");
                    }
                    else if (update.Message.Text == "/getchatid")
                    {
                        var escapedChatId = EscapeMarkdownV2(update.Message.Chat.Id.ToString());
                        
                        await _botClient.SendMessage(
                            chatId: update.Message.Chat.Id,
                            text: $"🆔 Ваш Chat ID: `{escapedChatId}`\n\n" +
                                  "Добавьте этот ID в appsettings\\.json в раздел AdminChatIds",
                            parseMode: ParseMode.MarkdownV2);
                    }
                    else
                    {
                        await _botClient.SendMessage(
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
        
        private string EscapeMarkdownV2(string text)
        {
            var charactersToEscape = new[] { '_', '*', '[', ']', '(', ')', '~', '`', '>', '#', '+', '-', '=', '|', '{', '}', '.', '!' };
            
            foreach (var ch in charactersToEscape)
            {
                text = text.Replace(ch.ToString(), $"\\{ch}");
            }
            
            return text;
        }
    }
}