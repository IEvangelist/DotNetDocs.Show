using System;
using System.Threading.Tasks;
using DotNetDocs.Services.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tweetinvi;
using Tweetinvi.Models;

namespace DotNetDocs.Services
{
    public class TwitterService
    {
        readonly TwitterOptions? _twitterSettings;
        readonly ILogger<TwitterService> _logger;
        readonly TwitterClient _twitterClient;

        public TwitterService(
            IOptions<TwitterOptions> options,
            ILogger<TwitterService> logger)
        {
            _twitterSettings =
                options.Value ?? throw new ArgumentNullException(
                    nameof(options), "Twitter options are required.");
            _logger = logger;

            _twitterClient = new TwitterClient(
                _twitterSettings.ConsumerKey,
                _twitterSettings.ConsumerSecret,
                _twitterSettings.AccessToken,
                _twitterSettings.AccessTokenSecret);
        }

        public async Task<string?> GetUserProfileImageAsync(string twitterHandle)
        {
            try
            {
                IUser user = await _twitterClient.Users.GetUserAsync(twitterHandle);
                return user?.ProfileImageUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }

            return default;
        }
    }
}
