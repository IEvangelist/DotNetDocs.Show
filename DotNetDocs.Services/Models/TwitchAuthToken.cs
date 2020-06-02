using Newtonsoft.Json;

namespace DotNetDocs.Services.Models
{
    public class TwitchAuthToken
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; } = null!;

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; } = null!;

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("scope")]
        public string[] Scopes { get; set; } = null!;

        [JsonProperty("token_type")]
        public string TokenType { get; set; } = null!;
    }
}
