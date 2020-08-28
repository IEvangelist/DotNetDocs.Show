using System;

namespace DotNetDocs.Extensions
{
    public static class DateTimeExtensions
    {
        // https://stackoverflow.com/a/6346190/2410379
        public static DateTime GetNextWeekday(this DateTime start, DayOfWeek day)
        {
            // The (... + 7) % 7 ensures we end up with a value in the range [0, 6]
            int daysToAdd = ((int)day - (int)start.DayOfWeek + 7) % 7;
            return start.AddDays(daysToAdd);
        }

        // https://stackoverflow.com/a/54713397/2410379
        public static bool IsBetween(this DateTime source, DateTime start, DateTime end)
            => source >= start && source <= end;
    }
}
