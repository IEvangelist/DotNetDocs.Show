using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetDocs.Services.Options;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Microsoft.Extensions.Options;

namespace DotNetDocs.Services
{
    public class YouTubeVideoService
    {
        readonly YouTubeService _youTubeService;

        public YouTubeVideoService(IOptions<YouTubeOptions> options)
        {
            _youTubeService =
                new YouTubeService(
                    new BaseClientService.Initializer()
                    {
                        ApiKey = options.Value.ApiKey,
                        ApplicationName = "TheDotNetDocsShow"
                    });
        }

        public async Task<IList<Video>> GetVideosAsync(params string[] videoIds)
        {
            var list = _youTubeService.Videos.List(new[] { "snippet", "statistics" });
            list.Id = videoIds;
            list.MaxResults = 52; // Limit to a year worth of shows.

            var videoResponse = await list.ExecuteAsync();
            return videoResponse.Items;
        }
    }
}
