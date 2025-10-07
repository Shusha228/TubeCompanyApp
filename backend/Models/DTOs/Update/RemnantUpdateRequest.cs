using System.Text.Json.Serialization;
namespace backend.Models.DTOs.Update
{
    public class RemnantUpdateRequest
    {
        [JsonPropertyName("ArrayOfRemnantsEl")]
        public List<RemnantUpdateDto> ArrayOfRemnantsEl { get; set; } = new();
    }

}