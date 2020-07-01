using System;
using System.Runtime.InteropServices;

namespace DotNetDocs.Services
{
    public class DateTimeService
    {
        public TimeZoneInfo CentralTimeZone { get; private set; } =
            TimeZoneInfo.FindSystemTimeZoneById(
                RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                    ? "America/Chicago"
                    : "Central Standard Time");
    }
}
