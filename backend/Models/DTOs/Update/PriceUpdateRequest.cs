using System.Text.Json.Serialization;
namespace backend.Models.DTOs.Update
{
    public class PriceUpdateRequest
    {
        [JsonPropertyName("ArrayOfPricesEl")]
        public List<PriceUpdateDto> ArrayOfPriceEl { get; set; } = new();
    }
}