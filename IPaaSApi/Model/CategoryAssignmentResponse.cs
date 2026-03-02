using Newtonsoft.Json;

namespace Integration.Data.IPaaSApi.Model
{
    /// <summary>
    /// Represents a category assignment nested within a product or variant response.
    /// </summary>
    public class CategoryAssignmentResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("category_id")]
        public long CategoryId { get; set; }

        [JsonProperty("category_name")]
        public string CategoryName { get; set; }
    }
}
