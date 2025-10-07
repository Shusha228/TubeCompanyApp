using System.ComponentModel;
using backend.Models.Entities;

namespace backend.Models.DTOs.Order
{
    public class CreateOrderRequest
    {
        [Description("Идентификатор пользователя в Telegram")]
        public long TelegramUserId { get; set; }

        [Description("Список товаров в заказе")]
        public List<CartItem> Items { get; set; } = new();

        [Description("Информация о клиенте")]
        public CustomerInfo CustomerInfo { get; set; } = new();
    }
}