using System.ComponentModel;
using backend.Controllers;
using backend.Models.Entities;

namespace backend.Models.DTOs.Cart
{
    public class CartPaginationResponse
    {
        [Description("Список товаров в корзине")]
        public List<CartItem> Items { get; set; } = new List<CartItem>();

        [Description("Метаданные пагинации")]
        public PaginationMeta Meta { get; set; } = new PaginationMeta();
    }
}