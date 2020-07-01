using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using DotNetDocs.Services.Models;
using Microsoft.AspNetCore.Components;

namespace DotNetDocs.Web.Extensions
{
    public static class ShowExtensions
    {
        public static string ToDateString(this DocsShow show)
        {
            if (!show.IsScheduled)
            {
                return "TBD";
            }

            // TODO: fix this
            var dateTime = show.Date!.Value;
            var timeSpan = DateTimeOffset.Now.Subtract(dateTime);
            var dayDiff = (int)timeSpan.TotalDays;
            var secDiff = (int)timeSpan.TotalSeconds;

            if (dayDiff < 0 || dayDiff >= 31)
            {
                return $"{dateTime:MMM dd, yyyy}";
            }

            static string CalculateDayString(DateTimeOffset dateTime, int days, int seconds) => days switch
            {
                0 => CalculateSecString(dateTime, seconds),
                1 => "Yesterday",
                _ when (2..7).IsInRange(days) => string.Format("{0} days ago", days),
                _ when (8..31).IsInRange(days) => string.Format("{0} weeks ago", Math.Ceiling((double)days / 7)),
                _ => $"{dateTime:MMM dd, yyyy}"
            };

            static string CalculateSecString(DateTimeOffset dateTime, int seconds) => seconds switch
            {
                _ when (0..60).IsInRange(seconds) => "Just now",
                _ when (61..120).IsInRange(seconds) => "1 minute ago",
                _ when (121..3600).IsInRange(seconds) => string.Format("{0} minutes ago", Math.Floor((double)seconds / 60)),
                _ when (3601..7200).IsInRange(seconds) => "1 hour ago",
                _ when (7201..86400).IsInRange(seconds) => string.Format("{0} hours ago", Math.Floor((double)seconds / 3600)),
                _ => $"{dateTime:MMM dd, yyyy}"
            };

            return CalculateDayString(dateTime, dayDiff, secDiff);
        }

        public static bool IsInRange(this Range range, int value) =>
            value >= range.Start.Value && value <= range.End.Value;

        public static string AddToGoogleCalendar(this DocsShow show)
        {
            var date = show.Date!.Value;

            // Reference: http://stackoverflow.com/a/21653600/22941
            static string UrlEncode(string value) => UrlEncoder.Default.Encode(value);

            var from = UrlEncode($"{date:yyyyMMddTHHmmss}");
            var to = UrlEncode($"{date.AddHours(1):yyyyMMddTHHmmss}");
            var text = UrlEncode($"The .NET docs show - live with {show.Guests.ToCommaSeparatedString(false)}.");
            var location = UrlEncode("https://www.twitch.tv/thedotnetdocs");
            var zone = UrlEncode("America/Chicago");

            return $"https://www.google.com/calendar/render?action=TEMPLATE&ctz={zone}&text={text}&dates={from}/{to}&details={location}&location={location}&sf=true&output=xml";
        }

        internal static MarkupString ToGeneralDescription(this DocsShow show) =>
            new MarkupString($"Join {show.Guests.ToCommaSeparatedString()}, along with hosts {show.Hosts.ToCommaSeparatedString(false)} for a captivating conversation about .NET.");

        public static string ToCommaSeparatedString(this IEnumerable<Person> persons, bool includeTwitter = true) =>
            string.Join(", ", persons.Select(p => p.ToPrintFriendlyName(includeTwitter)));

        public static string ToPrintFriendlyName(this Person person, bool includeTwitter = true)
        {
            string? twitterHandle = person.TwitterHandle is null || !includeTwitter ? null : $" ({person.TwitterHandle.ToTwitterUrl()})";
            return $"{person.FirstName} {person.LastName}{twitterHandle}";
        }

        public static string ToTwitterUrl(this string twitterHandle)
        {
            string? name = twitterHandle.Substring(1);
            return $"<a href=\"https://twitter.com/{name}\" target=\"_blank\">{twitterHandle} <span class='oi oi-sm oi-external-link'></span></a>";
        }

        internal static MarkupString ToTwitterMarkupString(this string twitterHandle) =>
            new MarkupString(twitterHandle.ToTwitterUrl());

        public static string ToMvpUrl(this Person person) =>
            $"<a href='https://mvp.microsoft.com/en-us/PublicProfile/{person.MicrosoftMvpId}' target='_blank'>MVP</a>";
    }
}
