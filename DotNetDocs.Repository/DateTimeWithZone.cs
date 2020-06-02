using System;

namespace DotNetDocs.Repository
{
    public struct DateTimeWithZone
    {
        public DateTimeWithZone(DateTime dateTime, TimeZoneInfo timeZone)
        {
            var dateTimeUnspec = DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);
            UniversalTime = TimeZoneInfo.ConvertTimeToUtc(dateTimeUnspec, timeZone);
            TimeZone = timeZone;
        }

        public DateTime UniversalTime { get; }

        public TimeZoneInfo TimeZone { get; }

        public DateTime LocalTime => TimeZoneInfo.ConvertTime(UniversalTime, TimeZone);
    }
}
