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
        private readonly TelegramBotClient _botClient;
        private readonly string _baseUrl;

        public TelegramService(IConfiguration configuration)
        {
            var botToken = configuration["Telegram:BotToken"];
            _botClient = new TelegramBotClient(botToken);
            _baseUrl = configuration["App:BaseUrl"];
        }

        public async Task HandleUpdateAsync(Update update)
        {
            if (update.Message?.Text == "/start")
            {
                var webAppUrl = $"{_baseUrl}/miniapp";
                
                var keyboard = new InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithWebApp(
                            "🏭 Открыть каталог труб",
                            new WebAppInfo { Url = webAppUrl }
                        )
                    }
                });

                await _botClient.SendMessage(
                    update.Message.Chat.Id,
                    "🏭 Добро пожаловать в каталог трубной продукции!\n\n" +
                    "Здесь вы можете:\n" +
                    "• 📋 Просматривать каталог продукции\n" +
                    "• 🔍 Фильтровать трубы по параметрам\n" +
                    "• 🛒 Формировать заказы с автоматическими скидками\n" +
                    "• 💰 Видеть актуальные цены и остатки");
            }
        }
    }
}