using System.ComponentModel;

namespace backend.Models.DTOs.MiniApp
{

    public class TelegramValidationResult
    {
        [Description("Результат валидации (true - успешно, false - ошибка)")]
        public bool IsValid { get; set; }

        [Description("Сообщение об ошибке (если есть)")]
        public string Error { get; set; } = string.Empty;

        [Description("Идентификатор пользователя Telegram")]
        public string UserId { get; set; } = string.Empty;

        [Description("Имя пользователя Telegram")]
        public string FirstName { get; set; } = string.Empty;

        [Description("Фамилия пользователя Telegram")]
        public string LastName { get; set; } = string.Empty;
    }
}