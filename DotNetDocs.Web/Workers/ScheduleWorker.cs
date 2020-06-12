using System;
using System.Threading;
using System.Threading.Tasks;
using DotNetDocs.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;

namespace DotNetDocs.Web.Workers
{
    public class ScheduleWorker : BackgroundService
    {
        readonly IMemoryCache _cache;
        readonly IScheduleService _scheduleService;

        static readonly TimeSpan AbsoluteExpiration = TimeSpan.FromHours(12);
        static readonly TimeSpan SlidingExpiration = TimeSpan.FromHours(2);
        static readonly TimeSpan FullCycleDelay = TimeSpan.FromHours(6);

        MemoryCacheEntryOptions CacheEntryOptions => new MemoryCacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = AbsoluteExpiration,
            SlidingExpiration = SlidingExpiration
        };

        public ScheduleWorker(IMemoryCache cache, IScheduleService scheduleService)
        {
            _cache = cache;
            _scheduleService = scheduleService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Twenty previous weeks worth of episodes.
            var sinceDate = DateTime.Now.Date.AddDays(-(20 * 7));
            while (!stoppingToken.IsCancellationRequested)
            {
                DateTime evaluatedDate = default;
                try
                {
                    if (evaluatedDate == default || sinceDate != evaluatedDate)
                    {
                        var shows = await _scheduleService.GetAllAsync(sinceDate);
                        _cache.Set(CacheKeys.ShowSchedule, shows, CacheEntryOptions);
                        if (evaluatedDate > default(DateTime))
                        {
                            sinceDate = evaluatedDate;
                        }
                    }
                }
                finally
                {
                    evaluatedDate = DateTime.Now.Date.AddDays(-(20 * 7));
                    await Task.Delay(FullCycleDelay);
                }
            }
        }
    }
}
