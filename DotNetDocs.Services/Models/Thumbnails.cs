using System.Collections.Generic;
using Newtonsoft.Json;

namespace DotNetDocs.Services.Models
{
    public class Thumbnails
    {
        [JsonProperty("large")]
        public List<Thumbnail>? Large { get; set; }

        [JsonProperty("medium")]
        public List<Thumbnail>? Medium { get; set; }

        [JsonProperty("small")]
        public List<Thumbnail>? Small { get; set; }

        [JsonProperty("template")]
        public List<Thumbnail>? Template { get; set; }
    }
}
