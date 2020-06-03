using Newtonsoft.Json;

namespace DotNetDocs.Services.Models
{
    public class Thumbnail
    {
        [JsonProperty("type")]
        public string? Type { get; set; }

        [JsonProperty("url")]
        public string? Url { get; set; }
    }
}
