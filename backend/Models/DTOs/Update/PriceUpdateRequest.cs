using System.ComponentModel;
using System.Text.Json.Serialization;

namespace backend.Models.DTOs.Update
{
    public class PriceUpdateRequest
    {
        [Description("Массив элементов цен для обновления")]
        [JsonPropertyName("ArrayOfPricesEl")]
        public List<PriceUpdateDto> ArrayOfPriceEl { get; set; } = new();
    }
}