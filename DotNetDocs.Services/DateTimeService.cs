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

        public bool IsHoliday(DateTime date)
        {
            if (DateSystem.IsPublicHoliday(date, CountryCode.US | CountryCode.CA))
            {
                return true;
            }

            // No shows for Thanksgiving week.
            var thanksgivingWeekMonday = DateSystem.FindDay(date.Year, 11, DayOfWeek.Monday, 4);
            var thanksgivingWeekFriday = DateSystem.FindDay(date.Year, 11, DayOfWeek.Friday, 4);
            if (date.IsBetween(thanksgivingWeekMonday, thanksgivingWeekFriday))
            {
                return true;
            }

            // True if Christmas eve or New Year's eve, else false
            if (date.Month == 12 && date.Day.IsInRange(20..31))
            {
                return true;
            }

            return false;
        }

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
            var pastShows = orderedShows.Where(show => show.Date!.Value.DateTime <= start).Take(20);
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
