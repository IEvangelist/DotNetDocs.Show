using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetDocs.Services;
using DotNetDocs.Services.Models;
using DotNetDocs.Web.Extensions;
using DotNetDocs.Web.Options;
using DotNetDocs.Web.Workers;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

namespace DotNetDocs.Web.Pages
{
    public partial class Index
    {
        [Inject]
        public IMemoryCache? Cache { get; set; }

        [Inject]
        public IScheduleService? ScheduleService { get; set; }

        [Inject]
        public DateTimeService DateTimeService { get; set; } = null!;

        [Inject]
        public IJSRuntime? JavaScript { get; set; }

        [Inject]
        public IOptionsMonitor<FeatureOptions>? Features { get; set; }

        const string NextShowElementId = "next-show";

        private IEnumerable<DocsShow> _shows = null!;
        private IEnumerable<DocsShow> _pastShows = null!;
        private IEnumerable<object> _futureShows = null!;

        private bool _hasMoreShows;
        private DocsShow _nextShow = null!;
        private DateTime _now;

        protected override async Task OnInitializedAsync()
        {
            _now =
                TimeZoneInfo.ConvertTime(DateTime.Now, DateTimeService.CentralTimeZone);
            var dailyShowTime =
                new DateTimeOffset(
                    _now.Year, _now.Month, _now.Day, 11, 0, 0,
                    DateTimeService.CentralTimeZone.BaseUtcOffset);

            var shows = await Cache.GetOrCreateAsync(
                CacheKeys.ShowSchedule,
                async _ =>
                await ScheduleService!.GetAllAsync(_now.AddDays(-(20 * 7))));

            _shows = shows.Where(show => show.IsPublished);

            var orderedShows = _shows.OrderByDescending(show => show.Date);
            _pastShows = orderedShows.Where(show => show.Date <= dailyShowTime).Take(12);
            _hasMoreShows = orderedShows.Count() > _pastShows.Count();

            var futureShows = orderedShows.Where(show => show.Date > dailyShowTime);
            _nextShow = futureShows.TakeLast(1).SingleOrDefault();
            var scheduledShows = futureShows.SkipLast(1);
            const int nearestOfMultiple = 4;
            var count = scheduledShows.Count();

            if (Features?.CurrentValue?.InterleaveShowGaps ?? false)
            {
                _futureShows =
                    scheduledShows.InterleaveWithAdaptor(
                        show => show.Date!.Value,
                        date => date,
                        TimeSpan.FromDays(7),
                        nearestOfMultiple)
                    .OrderByDescending(showOrDate =>
                        showOrDate is DocsShow show ? show.Date!.Value : (DateTimeOffset)showOrDate);

                return;
            }

            (int remainder, int nearest) = count.RoundUpToNearest(nearestOfMultiple);
            _futureShows = scheduledShows.Take(Math.Max(4, remainder == 0 ? nearest : nearest - 4));
        }
    }
}
