using System.ComponentModel;
using System.Text.Json.Serialization;

namespace backend.Models.Telegram
{
    public class CreateTelegramUserRequest
    {
        [Description("ID пользователя в Telegram")]
        [JsonPropertyName("telegramUserId")]
        public long TelegramUserId { get; set; }

        [Description("Имя")]
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; } = string.Empty;

        [Description("Фамилия")]
        [JsonPropertyName("lastName")]
        public string LastName { get; set; } = string.Empty;

        [Description("ИНН")]
        [JsonPropertyName("inn")]
        public string Inn { get; set; } = string.Empty;

        [Description("Email")]
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [Description("Телефон")]
        [JsonPropertyName("phone")]
        public string Phone { get; set; } = string.Empty;

        [Description("Username в Telegram")]
        [JsonPropertyName("username")]
        public string? Username { get; set; }
    }
}