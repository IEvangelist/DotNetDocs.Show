using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DotNetDocs.Services.Models;
using DotNetDocs.Services.Options;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace DotNetDocs.Services
{
    public class TwitchService
    {
        const string TokenUrl = "https://id.twitch.tv/oauth2/token";
        const string VideoUrl = "https://api.twitch.tv/kraken/videos";

        readonly HttpClient _client;
        readonly TwitchOptions _twitchOptions;

        public TwitchService(HttpClient client, IOptions<TwitchOptions> options)
        {
            _client = client;
            _twitchOptions = options.Value;

            _client.DefaultRequestHeaders.Add("Accept", "application/vnd.twitchtv.v5+json");
            _client.DefaultRequestHeaders.Add("Client-ID", _twitchOptions.ClientId);
        }

        async ValueTask<string> GetOAuthTokenAsync()
        {
            using var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["client_id"] = _twitchOptions.ClientId,
                ["client_secret"] = _twitchOptions.ClientSecret,
                ["grant_type"] = "client_credentials",
            });

            var response = await _client.PostAsync(TokenUrl, content);
            var json = await response.Content.ReadAsStringAsync();
            var twitchAuthToken = JsonConvert.DeserializeObject<TwitchAuthToken>(json);

            return twitchAuthToken.AccessToken;
        }

        public async ValueTask<TwitchVideo> GetTwitchVideoAsync(int videoId)
        {
            string? json = await _client.GetStringAsync($"{VideoUrl}/{videoId}?{_twitchOptions.ToQueryString()}");
            return JsonConvert.DeserializeObject<TwitchVideo>(json);
        }
    }
}
