using System.ComponentModel;
using System.Text.Json.Serialization;

namespace backend.Models.DTOs.Price
{
    public class PaginatedResponse<T>
    {
        [Description("Данные на текущей странице")]
        [JsonPropertyName("data")]
        public List<T> Data { get; set; } = new List<T>();

        [Description("Текущая страница")]
        [JsonPropertyName("page")]
        public int Page { get; set; }

        [Description("Размер страницы (количество элементов на странице)")]
        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; }

        [Description("Общее количество элементов")]
        [JsonPropertyName("totalCount")]
        public int TotalCount { get; set; }

        [Description("Общее количество страниц")]
        [JsonPropertyName("totalPages")]
        public int TotalPages { get; set; }

        [Description("Есть ли следующая страница")]
        [JsonPropertyName("hasNextPage")]
        public bool HasNextPage { get; set; }

        [Description("Есть ли предыдущая страница")]
        [JsonPropertyName("hasPreviousPage")]
        public bool HasPreviousPage { get; set; }
    }
}