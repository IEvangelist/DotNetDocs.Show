using System;
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

        public TwitterService(
            IOptions<TwitterOptions> options,
            ILogger<TwitterService> logger)
        {
            _twitterSettings = options.Value;
            _logger = logger;

            if (_twitterSettings is null)
            {
                return;
            }

            Auth.SetUserCredentials(
                _twitterSettings.ConsumerKey,
                _twitterSettings.ConsumerSecret,
                _twitterSettings.AccessToken,
                _twitterSettings.AccessTokenSecret);
        }

        public string? GetUserProfileImage(string twitterHandle)
        {
            try
            {
                IUser? user = User.GetUserFromScreenName(twitterHandle);
                return user?.ProfileImageUrlHttps;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }

            return default;
        }
    }
}
