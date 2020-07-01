using System;
using System.Text.Json.Serialization;

namespace Blazing.ReCaptcha
{
    public class ReCaptchaVerificationResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        // ISO format yyyy-MM-dd'T'HH:mm:ssZZ
        [JsonPropertyName("challenge_ts")]
        public DateTimeOffset ChallengeTimestamp { get; set; }

        [JsonPropertyName("hostname")]
        public string Hostname { get; set; }

        [JsonPropertyName("error-codes")]
        public string[] ErrorCodes { get; set; } = new string[0];
    }
}
