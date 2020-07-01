using Xunit;
using DotNetDocs.Web.Extensions;
using System;
using System.Runtime.InteropServices;
using System.Linq;

namespace DotNetDocs.Web.Tests
{
    public class ExtensionTests
    {
        [
            Theory,
            InlineData(2, 7, 1, false),
            InlineData(2, 7, 2, true),
            InlineData(2, 7, 3, true),
            InlineData(2, 7, 4, true),
            InlineData(2, 7, 5, true),
            InlineData(2, 7, 6, true),
            InlineData(2, 7, 7, true),
            InlineData(2, 7, 8, false)
        ]
        public void IsInRangeTest(int start, int end, int number, bool expected) =>
            Assert.Equal(expected, (start..end).IsInRange(number));

        [
            Theory,
            InlineData(1, 4, 3, 4),
            InlineData(2, 4, 2, 4),
            InlineData(3, 4, 1, 4),
            InlineData(4, 4, 0, 4),
            InlineData(5, 4, 3, 8),
            InlineData(6, 4, 2, 8),
            InlineData(7, 4, 1, 8),
            InlineData(8, 4, 0, 8),
            InlineData(9, 4, 3, 12),
            InlineData(10, 4, 2, 12),
            InlineData(11, 4, 1, 12),
            InlineData(12, 4, 0, 12),
            InlineData(13, 4, 3, 16)
        ]
        public void RoundUpTest(int number, int roundTo, int expectedRemainder, int expectedNearest) =>
            Assert.Equal((expectedRemainder, expectedNearest), number.RoundUpToNearest(roundTo));

        [Fact]
        public void InterleaveWithAdaptor()
        {
            var centralTimezone = TimeZoneInfo.FindSystemTimeZoneById(
                RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                    ? "America/Chicago"
                    : "Central Standard Time");
            var offset = centralTimezone.BaseUtcOffset;
            var source = new[]
            {
                new DateTimeOffset(2020, 6, 30, 11, 0, 0, offset),
                new DateTimeOffset(2020, 7, 7, 11, 0, 0, offset),
                //..............   2020, 7, 14
                new DateTimeOffset(2020, 7, 21, 11, 0, 0, offset),
                //..............   2020, 7, 28
                new DateTimeOffset(2020, 8, 4, 11, 0, 0, offset),
                //..............   2020, 8, 11
                new DateTimeOffset(2020, 8, 18, 11, 0, 0, offset),
                new DateTimeOffset(2020, 8, 25, 11, 0, 0, offset)
            };

            var interleaved =
                source.InterleaveWithAdaptor(
                        date => date,
                        date => date,
                        TimeSpan.FromDays(7),
                        10)
                    .ToList();

            Assert.Equal(10, interleaved.Count);

            var result = interleaved.ToList();
            Assert.Equal(new DateTimeOffset(2020, 6, 30, 11, 0, 0, offset), result[0]);
            Assert.Equal(new DateTimeOffset(2020, 7,  7, 11, 0, 0, offset), result[1]);
            Assert.Equal(new DateTimeOffset(2020, 7, 14, 11, 0, 0, offset), result[2]);
            Assert.Equal(new DateTimeOffset(2020, 7, 21, 11, 0, 0, offset), result[3]);
            Assert.Equal(new DateTimeOffset(2020, 7, 28, 11, 0, 0, offset), result[4]);
            Assert.Equal(new DateTimeOffset(2020, 8,  4, 11, 0, 0, offset), result[5]);
            Assert.Equal(new DateTimeOffset(2020, 8, 11, 11, 0, 0, offset), result[6]);
            Assert.Equal(new DateTimeOffset(2020, 8, 18, 11, 0, 0, offset), result[7]);
            Assert.Equal(new DateTimeOffset(2020, 8, 25, 11, 0, 0, offset), result[8]);
            Assert.Equal(new DateTimeOffset(2020, 9,  1, 11, 0, 0, offset), result[9]);
        }
    }
}
