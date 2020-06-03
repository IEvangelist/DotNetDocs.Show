using System;
using Newtonsoft.Json;

namespace DotNetDocs.Services.Models
{

    public class TwitchVideo
    {
        [JsonProperty("_id")]
        public string Id { get; set; } = null!;

        [JsonProperty("broadcast_id")]
        public long BroadcastId { get; set; }

        [JsonProperty("broadcast_type")]
        public string? BroadcastType { get; set; }

        [JsonProperty("channel")]
        public TwitchChannel? Channel { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("description")]
        public string? Description { get; set; }

        [JsonProperty("description_html")]
        public string? DescriptionHtml { get; set; }

        [JsonProperty("language")]
        public string? Language { get; set; }

        [JsonProperty("length")]
        public int Length { get; set; }

        [JsonProperty("published_at")]
        public DateTime PublishedAt { get; set; }

        [JsonProperty("status")]
        public string? Status { get; set; }

        [JsonProperty("tag_list")]
        public string? TagList { get; set; }

        [JsonProperty("thumbnails")]
        public Thumbnails? Thumbnails { get; set; }

        [JsonProperty("title")]
        public string? Title { get; set; }

        [JsonProperty("url")]
        public string? Url { get; set; }

        [JsonProperty("views")]
        public int Views { get; set; }
    }
}
