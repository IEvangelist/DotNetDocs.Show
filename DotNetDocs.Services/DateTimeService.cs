using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using DotNetDocs.Extensions;
using DotNetDocs.Services.Models;
using Microsoft.Extensions.Logging;

namespace DotNetDocs.Services
{
    public class DateTimeService
    {
        readonly ILogger<DateTimeService> _logger;

        public TimeZoneInfo CentralTimeZone { get; private set; } =
            TimeZoneInfo.FindSystemTimeZoneById(
                RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                    ? "America/Chicago"
                    : "Central Standard Time");

        public DateTimeService(ILogger<DateTimeService> logger) => _logger = logger;

        public TimeSpan GetCentralTimeZoneOffset(DateTime date)
        {
            var daylightDelta = TimeSpan.Zero;
            if (IsDaylightSavingTime(date))
            {
                var relevantAdjustment =
                    CentralTimeZone.GetAdjustmentRules()
                                   .FirstOrDefault(a => a.DateStart < date && date < a.DateEnd);
                daylightDelta = relevantAdjustment?.DaylightDelta ?? TimeSpan.Zero;
            }
            var currentOffset = CentralTimeZone.BaseUtcOffset.Add(daylightDelta);

            return currentOffset;
        }

        public bool IsDaylightSavingTime(DateTime date) =>
            CentralTimeZone.IsDaylightSavingTime(date);

        public SegmentedShows GetSegmentedShows(
            IEnumerable<DocsShow> shows,
            DateTime segmentDate,
            bool? interleaveShowGaps = null)
        {
            _logger.LogInformation($"segmentDate: {segmentDate}");

            var start = TimeZoneInfo.ConvertTime(segmentDate, CentralTimeZone);
            _logger.LogInformation($"start: {start}");
            //var offset = GetCentralTimeZoneOffset(start);
            //var dailyShowEndTime = new DateTimeOffset(start.Year, start.Month, start.Day, 12, 0, 0, offset);

            var orderedShows = shows.OrderByDescending(show => show.Date);
            var pastShows = orderedShows.Where(show => show.Date!.Value.DateTime <= start).Take(12);
            var futureShows = orderedShows.Where(show => show.Date!.Value.DateTime > start);
            var nextShow = futureShows.TakeLast(1).SingleOrDefault();
            var scheduledShows = futureShows.SkipLast(1);

            const int nearestOfMultiple = 4;
            if (interleaveShowGaps ?? false)
            {
                futureShows =
                    scheduledShows.InterleaveWithAdaptor(
                        show => show.Date!.Value,
                        date => DocsShow.CreatePlaceholder(date),
                        TimeSpan.FromDays(7),
                        nearestOfMultiple)
                    .OrderByDescending(show => show.Date!.Value);

                return new SegmentedShows(pastShows, nextShow, futureShows);
            }

            var count = scheduledShows.Count();
            (int remainder, int nearest) = count.RoundUpToNearest(nearestOfMultiple);
            futureShows = scheduledShows.Take(Math.Max(4, remainder == 0 ? nearest : nearest - 4));

            return new SegmentedShows(pastShows, nextShow, futureShows);
        }
    }
}
