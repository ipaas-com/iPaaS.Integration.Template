using Newtonsoft.Json;

namespace Integration.Data.IPaaSApi.Model
{
    public class GenericResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
