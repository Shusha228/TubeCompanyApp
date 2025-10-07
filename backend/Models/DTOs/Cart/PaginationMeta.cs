using System.ComponentModel;

namespace backend.Models.DTOs.Cart
{
    public class PaginationMeta
    {
        [Description("Общее количество страниц")]
        public int TotalPages { get; set; }

        [Description("Текущая страница")]
        public int Page { get; set; }

        [Description("Лимит элементов на странице")]
        public int PageLimit { get; set; }

        [Description("Общее количество элементов")]
        public int TotalCount { get; set; }

        [Description("Поисковый запрос (если применимо)")]
        public string? SearchTerm { get; set; }
    }
}