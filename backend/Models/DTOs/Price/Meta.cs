
using System.Text.Json.Serialization;

namespace backend.Models.DTOs.Price
{
    public class Meta
    {
        [JsonPropertyName("totalPages")]
        public int TotalPages { get; set; }

        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("pageLimit")]
        public int PageLimit { get; set; }

        [JsonPropertyName("totalCount")]
        public int TotalCount { get; set; }
    }
}