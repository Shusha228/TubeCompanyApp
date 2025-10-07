using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace backend.Models.Entities
{
    /// <summary>
    /// Пользователь Telegram
    /// </summary>
    public class TelegramUser
    {
        /// <summary>
        /// ID пользователя в Telegram
        /// </summary>
        [Key]
        [Description("ID пользователя в Telegram")]
        [JsonPropertyName("telegramUserId")]
        public long TelegramUserId { get; set; }

        /// <summary>
        /// Имя пользователя
        /// </summary>
        [Description("Имя")]
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Фамилия пользователя
        /// </summary>
        [Description("Фамилия")]
        [JsonPropertyName("lastName")]
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// ИНН пользователя/компании
        /// </summary>
        [Description("ИНН")]
        [JsonPropertyName("inn")]
        public string Inn { get; set; } = string.Empty;

        /// <summary>
        /// Email пользователя
        /// </summary>
        [Description("Email")]
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Телефон пользователя
        /// </summary>
        [Description("Телефон")]
        [JsonPropertyName("phone")]
        public string Phone { get; set; } = string.Empty;

        /// <summary>
        /// Username в Telegram
        /// </summary>
        [Description("Username в Telegram")]
        [JsonPropertyName("username")]
        public string? Username { get; set; }

        /// <summary>
        /// Дата регистрации
        /// </summary>
        [Description("Дата регистрации")]
        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Дата последнего обновления
        /// </summary>
        [Description("Дата последнего обновления")]
        [JsonPropertyName("updatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Статус пользователя
        /// </summary>
        [Description("Статус")]
        [JsonPropertyName("status")]
        public string Status { get; set; } = "Active";
    }
}