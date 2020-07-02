using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetDocs.Services;
using DotNetDocs.Services.Models;
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

        const string? NextShowElementId = "next-show";

        private SegmentedShows? _segmentedShows;

        private IEnumerable<DocsShow>? _shows;
        private IEnumerable<DocsShow>? _pastShows;
        private IEnumerable<DocsShow>? _futureShows;

        private DocsShow? _nextShow;

        private readonly Lazy<MarkupString> _dateTimeDebug = new Lazy<MarkupString>(() =>
        {
            var builder = new StringBuilder();
            builder.AppendLine($"TimeZoneInfo.Local: {TimeZoneInfo.Local}");
            builder.AppendLine($"DateTime.Now: {DateTime.Now:MMM dd yyy hh:mm:ss zzzz}");
            builder.AppendLine($"DateTime.UtcNow: {DateTime.UtcNow:MMM dd yyy hh:mm:ss zzzz}");
            builder.AppendLine($"DateTimeOffset.Now: {DateTimeOffset.Now:MMM dd yyy hh:mm:ss zzzz}");
            builder.AppendLine($"DateTimeOffset.UtcNow: {DateTimeOffset.UtcNow:MMM dd yyy hh:mm:ss zzzz}");

            return new MarkupString(builder.ToString());
        });

        protected override async Task OnInitializedAsync()
        {
            var now = DateTime.Now;
            var shows = await Cache.GetOrCreateAsync(
                CacheKeys.ShowSchedule,
                async _ =>
                await ScheduleService!.GetAllAsync(now.AddDays(-(20 * 7))));

            _shows = shows.Where(show => show.IsPublished);

            _segmentedShows = DateTimeService.GetSegmentedShows(
                _shows,
                now,
                Features?.CurrentValue?.InterleaveShowGaps);

            _pastShows = _segmentedShows.PastShows;
            _nextShow = _segmentedShows.NextShow;
            _futureShows = _segmentedShows.FutureShows;
        }
    }
}
