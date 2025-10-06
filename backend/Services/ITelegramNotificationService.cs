using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using backend.Models;
using backend.Data;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public interface ITelegramNotificationService
    {
        Task NotifyAdminsAboutNewOrderAsync(Order order);
        Task SendMessageToAdminsAsync(string message, ParseMode? parseMode = null);
        Task<bool> IsAdminConfiguredAsync();
    }

    public class TelegramNotificationService : ITelegramNotificationService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<TelegramNotificationService> _logger;
        private readonly ApplicationDbContext _context;
        private readonly ITelegramBotClient _botClient;

        public TelegramNotificationService(
            IConfiguration configuration,
            ILogger<TelegramNotificationService> logger,
            ApplicationDbContext context,
            ITelegramBotClient botClient)
        {
            _configuration = configuration;
            _logger = logger;
            _context = context;
            _botClient = botClient;
        }

        public async Task NotifyAdminsAboutNewOrderAsync(Order order)
        {
            try
            {
                var message = FormatNewOrderMessage(order);
                await SendMessageToAdminsAsync(message, ParseMode.Html);
                
                await MarkOrderAsNotifiedAsync(order.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error notifying admins about new order");
            }
        }

        public async Task SendMessageToAdminsAsync(string message, ParseMode? parseMode = null)
        {
            var adminChatIds = GetAdminChatIds();
            var botToken = _configuration["Telegram:BotToken"];

            if (string.IsNullOrEmpty(botToken) || !adminChatIds.Any())
            {
                _logger.LogWarning("Bot token or admin chat IDs not configured");
                return;
            }

            foreach (var chatId in adminChatIds)
            {
                try
                {
                    await _botClient.SendMessage(
                        chatId: chatId,
                        text: message,
                        parseMode: parseMode ?? default);
            
                    _logger.LogInformation($"Notification sent to admin chat: {chatId}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error sending message to admin chat: {chatId}");
                }
            }
        }

        public async Task<bool> IsAdminConfiguredAsync()
        {
            var adminChatIds = GetAdminChatIds();
            return !string.IsNullOrEmpty(_configuration["Telegram:BotToken"]) && adminChatIds.Any();
        }

        private List<long> GetAdminChatIds()
        {
            var adminChatIds = _configuration.GetSection("Telegram:AdminChatIds").Get<List<long>>();
            return adminChatIds ?? new List<long>();
        }

        private string FormatNewOrderMessage(Order order)
        {
            var itemsText = string.Join("\n", order.Items.Select((item, index) =>
                $"{index + 1}. {item.ProductName}\n" +
                $"   Количество: {item.Quantity} {(item.IsInMeters ? "м" : "т")}\n" +
                $"   Цена за ед.: {item.UnitPrice:0.00} ₽\n" +
                $"   Сумма: {item.FinalPrice:0.00} ₽"));

            return $"🆕 <b>НОВЫЙ ЗАКАЗ #{order.Id}</b>\n\n" +
                   $"👤 <b>Клиент:</b> {order.FirstName} {order.LastName}\n" +
                   $"📞 <b>Телефон:</b> {order.Phone}\n" +
                   $"📧 <b>Email:</b> {order.Email}\n" +
                   $"🏢 <b>ИНН:</b> {order.Inn}\n\n" +
                   $"🛒 <b>Состав заказа:</b>\n{itemsText}\n\n" +
                   $"💰 <b>Общая сумма:</b> {order.TotalAmount:0.00} ₽\n" +
                   $"📅 <b>Дата заказа:</b> {order.CreatedAt:dd.MM.yyyy HH:mm}\n" +
                   $"🔗 <b>ID заказа:</b> <code>{order.Id}</code>";
        }

        private async Task MarkOrderAsNotifiedAsync(string orderId)
        {
            try
            {
                var order = await _context.Orders.FindAsync(orderId);
                if (order != null)
                {
                    order.AdminNotified = true;
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error marking order {orderId} as notified");
            }
        }
    }
}