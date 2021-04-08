using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DotNetDocs.Services;
using Google.Apis.YouTube.v3.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace DotNetDocs.Web.Pages
{
    public partial class Stats
    {
        static readonly Regex YouTubeVideoRegex =
            new Regex(@"https://www\.youtube\.com/watch\?v=(?<Id>.+)");

        private IList<Video> _videos = null!;

        [Inject]
        public YouTubeVideoService YouTube { get; set; } = null!;

        [Inject]
        public NavigationManager Navigation { get; set; } = null!;

        [Inject]
        public IScheduleService? ScheduleService { get; set; }

        protected StatsModel Model { get; set; } = null!;
        protected bool IsLoading { get; set; }
        protected int NumberOfVideos { get; set; }
        protected long TotalWeeks { get; set; }
        protected long TotalViews { get; set; }
        protected long AverageViews { get; set; }
        protected long TotalLikes { get; set; }

        protected override void OnInitialized()
        {
            // Six months of show data
            Model = new StatsModel
            {
                SinceDate = DateTime.Now.Date.AddMonths(-6)
            };
        }

        protected async ValueTask RequestStatsAsync(EditContext context)
        {
            try
            {
                IsLoading = true;

                if (YouTube != null && ScheduleService != null)
                {
                    TotalWeeks = (long)((DateTime.Now.Date - Model.SinceDate).TotalDays / 7);
                    var shows = await ScheduleService.GetAllAsync(Model.SinceDate);

                    static string? ParseYouTubeVideoId(string url)
                    {
                        var match = YouTubeVideoRegex.Match(url);
                        return match.Success && match.Groups.ContainsKey("Id")
                            ? match.Groups["Id"].Value
                            : null;
                    }

                    var showYouTubeVideoIds =
                        shows.Select(show => ParseYouTubeVideoId(show.Url))
                             .Where(id => id != null)
                             .Select(id => id!)
                             .ToArray();

                    if (showYouTubeVideoIds?.Length > 0)
                    {
                        _videos = await YouTube.GetVideosAsync(showYouTubeVideoIds);

                        NumberOfVideos = _videos.Count;
                        TotalViews =
                            _videos.Where(v => v.Statistics.ViewCount.HasValue)
                                .Select(v => (long)v.Statistics.ViewCount!.Value)
                                .Sum();

                        TotalLikes =
                            _videos.Where(v => v.Statistics.LikeCount.HasValue)
                                .Select(v => (long)v.Statistics.LikeCount!.Value)
                                .Sum();

                        if (NumberOfVideos > 0)
                        {
                            AverageViews = TotalViews / NumberOfVideos;
                        }
                    }
                }
            }
            finally
            {
                IsLoading = false;
            }
        }


        protected void NavigateBack()
        {
            if (Navigation == null)
            {
                return;
            }

            Navigation.NavigateTo("/");
        }
    }

    public class StatsModel
    {
        public DateTime SinceDate { get; set; }
    }
}
