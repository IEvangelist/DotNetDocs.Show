using System;
using System.Linq;
using DotNetDocs.Services;
using DotNetDocs.Services.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace DotNetDocs.Web.Tests
{
    public class DateTimeServiceTests
    {
        readonly DateTimeService _systemUnderTest;

        public DateTimeServiceTests(ITestOutputHelper outputHelper)
        {
            _systemUnderTest = new ServiceCollection()
                .AddLogging(builder => builder.AddXUnit(outputHelper))
                .AddSingleton<DateTimeService>()
                .BuildServiceProvider()
                .GetRequiredService<DateTimeService>();
        }

        [Fact]
        public void GetSegmentedShowsCorrectlySegmentsWhenDaysOff()
        {
            var utcDate = new DateTimeOffset(2020, 5, 29, 11, 0, 0, TimeSpan.FromHours(-5));
            var startDate = _systemUnderTest.ConvertFromUtc(utcDate.UtcDateTime);
            var shows = new[]
            {
                DocsShow.CreatePlaceholder(startDate),
                DocsShow.CreatePlaceholder(startDate.AddDays(7)),
                DocsShow.CreatePlaceholder(startDate.AddDays(14)),
                DocsShow.CreatePlaceholder(startDate.AddDays(21)),
                DocsShow.CreatePlaceholder(startDate.AddDays(28))
            };

            var segmentedShows =
                _systemUnderTest.GetSegmentedShows(shows, utcDate.UtcDateTime.AddDays(15), true, 4);

            Assert.NotNull(segmentedShows);
            Assert.NotNull(segmentedShows.PastShows);
            Assert.NotNull(segmentedShows.NextShow);
            Assert.NotNull(segmentedShows.FutureShows);

            Assert.Equal(startDate.AddDays(21), segmentedShows.NextShow.Date);
        }

        [Fact]
        public void GetSegmentedShowsCorrectlySegments()
        {
            // Our next show, is the nearest show in the future - that is currently showing and hasn't ended.
            // Future shows, are all remaining shows in the future... (but not the next show)
            // Past shows are all shows in the past.

            var utcDate = new DateTimeOffset(2020, 5, 29, 11, 0, 0, TimeSpan.FromHours(-5));
            var startDate = _systemUnderTest.ConvertFromUtc(utcDate.UtcDateTime);
            var shows = new[]
            {
                new DocsShow { Date = startDate, Title = "First show" },
                new DocsShow { Date = startDate.AddDays(7), Title = "Second show" },
                new DocsShow { Date = startDate.AddDays(14), Title = "Third show" },
                new DocsShow { Date = startDate.AddDays(21), Title = "Fourth show" },
                new DocsShow { Date = startDate.AddDays(28), Title = "Fifth show" },
                new DocsShow { Date = startDate.AddDays(42), Title = "Seventh show" },
            };

            var segmentedShows =
                _systemUnderTest.GetSegmentedShows(
                    shows, utcDate.UtcDateTime.AddDays(7).AddSeconds(-7), true, 4);

            Assert.NotNull(segmentedShows);
            Assert.Single(segmentedShows.PastShows);
            Assert.Equal("Second show", segmentedShows.NextShow.Title);
            Assert.Equal(8, segmentedShows.FutureShows.Count());
            Assert.Contains(segmentedShows.FutureShows, s => s.IsPlaceholder);

            segmentedShows =
                _systemUnderTest.GetSegmentedShows(
                    shows, utcDate.UtcDateTime.AddDays(7), false, 4);

            Assert.NotNull(segmentedShows);
            Assert.Equal(2, segmentedShows.PastShows.Count());
            Assert.Equal("Third show", segmentedShows.NextShow.Title);
            Assert.Equal(3, segmentedShows.FutureShows.Count());
            Assert.DoesNotContain(segmentedShows.FutureShows, s => s.IsPlaceholder);
        }
    }
}
