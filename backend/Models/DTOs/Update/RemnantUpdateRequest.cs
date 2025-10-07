using System.ComponentModel;
using System.Text.Json.Serialization;

namespace backend.Models.DTOs.Update
{
    public class RemnantUpdateRequest
    {
        [Description("Массив элементов остатков для обновления")]
        [JsonPropertyName("ArrayOfRemnantsEl")]
        public List<RemnantUpdateDto> ArrayOfRemnantsEl { get; set; } = new();
    }
}