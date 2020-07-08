using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetDocs.Services;
using DotNetDocs.Services.Models;
using DotNetDocs.Web.Workers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Caching.Memory;

namespace DotNetDocs.Web.Pages
{
    public partial class SchedulePage
    {
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

        public IEnumerable<DocsShow>? Shows { get; private set; }

        bool _orderDescending = true;

        protected override async Task OnInitializedAsync()
        {
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

                Shows = segmentedShows.FutureShows;
            }
        }
    }
}
