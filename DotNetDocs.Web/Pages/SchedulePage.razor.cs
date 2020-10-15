using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetDocs.Services;
using DotNetDocs.Services.Models;
using DotNetDocs.Web.Extensions;
using DotNetDocs.Web.Interop;
using DotNetDocs.Web.PageModels;
using DotNetDocs.Web.Workers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.JSInterop;

namespace DotNetDocs.Web.Pages
{
    public partial class SchedulePage
    {
        [Inject]
        public IJSRuntime JavaScript { get; set; } = null!;

        [Inject]
        public NavigationManager Navigation { get; set; } = null!;

        [Inject]
        public IMemoryCache? Cache { get; set; }

        [Inject]
        public IScheduleService? ScheduleService { get; set; }

        [Inject]
        public IDataProtectionProvider? ProtectionProvider { get; set; }

        [Inject]
        public DateTimeService DateTimeService { get; set; } = null!;

        protected IEnumerable<DocsShow>? FilteredShows => _filterOption switch
        {
            FilterOption.AllShows => _shows,
            FilterOption.OnlyMicrosoft => _shows.Where(s => s.Guests.Any(g => g.IsBlueBadge)),
            FilterOption.OnlyMvps => _shows.Where(s => s.Guests.Any(g => g.IsMicrosoftMvp)),
            FilterOption.OnlyRequestable => _shows.Where(s => s.IsPlaceholder),
            _ => null
        };

        IEnumerable<DocsShow>? _shows;
        FilterOption _filterOption = FilterOption.AllShows;
        bool _orderDescending = false;

        protected override async Task OnInitializedAsync()
        {
            Navigation.TryGetQueryString("filter", out _filterOption);

            if (Cache != null && ScheduleService != null)
            {
                var utcNow = DateTime.UtcNow;
                var centralTimeNow = DateTimeService.ConvertFromUtc(utcNow);
                var shows = await Cache.GetOrCreateAsync(
                    CacheKeys.ShowSchedule,
                    async _ =>
                    await ScheduleService!.GetAllAsync(centralTimeNow.AddYears(-1)));
                
                var segmentedShows = DateTimeService.GetSegmentedShows(
                    shows.Where(show => show.IsPublished),
                    utcNow,
                    true,
                    12);

                _shows = segmentedShows.FutureShows.Concat(new[] { segmentedShows.NextShow! });
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender) =>
            await JavaScript.LoadTwitterImagesAsync();

        void UpdateFilter(FilterOption filter)
        {
            _filterOption = filter;
            Navigation.NavigateTo($"schedule?filter={_filterOption}");
        }

        string ToDisplayName(FilterOption option) => option switch
        {
            FilterOption.AllShows => "All shows",
            FilterOption.OnlyMicrosoft => "Microsoft Employees",
            FilterOption.OnlyMvps => "Microsoft MVPs",
            FilterOption.OnlyRequestable => "Available dates",
            _ => throw new Exception("WTF?!")
        };
    }
}
