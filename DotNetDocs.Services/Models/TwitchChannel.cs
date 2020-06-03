using Newtonsoft.Json;

namespace DotNetDocs.Services.Models
{
    public class TwitchChannel
    {
        [JsonProperty("_id")]
        public string? Id { get; set; }

        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("display_name")]
        public string? DisplayName { get; set; }
    }
}
