using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using DotNetDocs.Extensions;
using DotNetDocs.Services.Models;
using Microsoft.Extensions.Logging;
using Nager.Date;

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

        public bool IsHoliday(DateTime date) =>
            DateSystem.IsPublicHoliday(date, CountryCode.US) ||
            date.Month == 12 && date.Day == 24; // Christmas eve is a holiday too

        public bool IsDaylightSavingTime(DateTime date) =>
            CentralTimeZone.IsDaylightSavingTime(date);

        public DateTime ConvertFromUtc(DateTime date) =>
            TimeZoneInfo.ConvertTimeFromUtc(date, CentralTimeZone);

        public SegmentedShows GetSegmentedShows(
            IEnumerable<DocsShow> shows,
            DateTime segmentDate,
            bool? interleaveShowGaps = null,
            int nearestOfMultiple = 4)
        {
            _logger.LogInformation($"segmentDate: {segmentDate}");

            var start = ConvertFromUtc(segmentDate);
            _logger.LogInformation($"start: {start}");

            var orderedShows = shows.OrderByDescending(show => show.Date);
            var pastShows = orderedShows.Where(show => show.Date!.Value.DateTime <= start).Take(12);
            var futureShows = orderedShows.Where(show => show.Date!.Value.DateTime > start);
            var nextShow = futureShows.TakeLast(1).SingleOrDefault();
            var scheduledShows = futureShows.SkipLast(1);

            if (interleaveShowGaps ?? false)
            {
                DateTimeOffset AdjustForDaylightSavingsTime(DateTimeOffset dateTimeOffset)
                {
                    var result = dateTimeOffset.ToOffset(GetCentralTimeZoneOffset(dateTimeOffset.DateTime));
                    return result.Hour == 10 ? result.AddHours(1) : result;
                }

                futureShows =
                    scheduledShows.InterleaveWithAdaptor(
                        show => show.Date!.Value,
                        date => DocsShow.CreatePlaceholder(AdjustForDaylightSavingsTime(date)),
                        TimeSpan.FromDays(7),
                        nearestOfMultiple,
                        date => !IsHoliday(date.DateTime))
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
