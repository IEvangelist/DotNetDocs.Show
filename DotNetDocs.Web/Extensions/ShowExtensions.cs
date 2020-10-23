using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using DotNetDocs.Extensions;
using DotNetDocs.Services.Models;
using Microsoft.AspNetCore.Components;

namespace DotNetDocs.Web.Extensions
{
    public static class ShowExtensions
    {
        private enum LinkType
        {
            Unavailable,
            DevTo
        }

        internal static (bool isSuccessful, string? url, MarkupString icon)
            TryGetTooLongDontReadUrl(this DocsShow show)
        {
            string? tldrUrl = show?.TldrUrl;
            if (show is null || string.IsNullOrWhiteSpace(tldrUrl))
            {
                return (false, null, new MarkupString(null));
            }

            var uri = new Uri(tldrUrl);
            static LinkType GetLinkType(string host) => host.ToLower() switch
            {
                "dev.to" => LinkType.DevTo,

                _ => throw new NotImplementedException(),
            };

            static string GetFontAwesomeIcon(LinkType type) => type switch
            {
                LinkType.DevTo => "<i class='fab fa-dev'></i>",

                _ => throw new NotImplementedException(),
            };

            LinkType type = GetLinkType(uri.Host);
            return (true, show.TldrUrl, new MarkupString(GetFontAwesomeIcon(type)));
        }

        private enum EmbedKind
        {
            YouTube,
            Twitch
        }

        public static string ToEmbedUrl(this DocsShow show)
        {
            var uri = new Uri(show.Url);
            static EmbedKind GetEmbedKind(string host) => host.ToLower() switch
            {
                "www.youtube.com" => EmbedKind.YouTube,
                "www.twitch.tv" => EmbedKind.Twitch,

                _ => throw new NotImplementedException(),
            };

            return GetEmbedKind(uri.Host) switch
            {
                EmbedKind.Twitch => $"https://clips.twitch.tv/embed?clip={uri.Segments[^1]}&parent=dotnetdocs.dev",
                _ => $"https://www.youtube.com/embed/{uri.Query.Split("=")[1]}"
            };
        }

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
                _ when (2..7).Contains(days) => string.Format("{0} days ago", days),
                _ when (8..31).Contains(days) => string.Format("{0} weeks ago", Math.Ceiling((double)days / 7)),
                _ => $"{dateTime:MMM dd, yyyy}"
            };

            static string CalculateSecString(DateTimeOffset dateTime, int seconds) => seconds switch
            {
                _ when (0..60).Contains(seconds) => "Just now",
                _ when (61..120).Contains(seconds) => "1 minute ago",
                _ when (121..3600).Contains(seconds) => string.Format("{0} minutes ago", Math.Floor((double)seconds / 60)),
                _ when (3601..7200).Contains(seconds) => "1 hour ago",
                _ when (7201..86400).Contains(seconds) => string.Format("{0} hours ago", Math.Floor((double)seconds / 3600)),
                _ => $"{dateTime:MMM dd, yyyy}"
            };

            return CalculateDayString(dateTime, dayDiff, secDiff);
        }

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

        internal static string ToRssDescription(this DocsShow show)
        {
            var builder = new StringBuilder();
            if (show.IsInFuture)
            {
                builder.Append(
                    $"Join {show.Guests.ToCommaSeparatedString(false)}, along with hosts {show.Hosts.ToCommaSeparatedString(false)} for a captivating conversation about .NET.");
            }
            else
            {
                builder.Append(
                        $"In this episode of The .NET Docs Show {show.Guests.ToCommaSeparatedString(false)} discussed \"{show.Title}\". ");

                if (!string.IsNullOrWhiteSpace(show.TldrUrl))
                {
                    builder.Append($"Show details: {show.TldrUrl}. ");
                }

                if (!string.IsNullOrWhiteSpace(show.Url))
                {
                    builder.Append($"Show recording: {show.Url}.");
                }
            }

            return builder.ToString();
        }
            

        internal static MarkupString ToGeneralDescription(this DocsShow show) =>
            new MarkupString($"Join {show.Guests.ToCommaSeparatedString()}, along with hosts {show.Hosts.ToCommaSeparatedString(false)} for a captivating conversation about .NET.");

        public static string ToCommaSeparatedString(this IEnumerable<Person> persons, bool includeTwitter = true) =>
            string.Join(", ", persons.Select(p => p.ToPrintFriendlyName(includeTwitter)));

        public static string ToPrintFriendlyName(this Person person, bool includeTwitter = true)
        {
            string? twitterHandle = person.TwitterHandle is null || !includeTwitter ? null : $" {person.TwitterHandle.ToTwitterUrl()}";
            return $"{person.FirstName} {person.LastName}{twitterHandle}";
        }

        public static string ToTwitterUrl(this string? twitterHandle)
        {
            string? name = (twitterHandle?.StartsWith("@") ?? false) ? twitterHandle.Substring(1) : twitterHandle;
            return $"<a href=\"https://twitter.com/{name ?? "dotnetdocsshow"}\" target=\"_blank\">{twitterHandle ?? "dotnetdocsshow"} <i class='fas fa-sm fa-external-link-square-alt'></i></a>";
        }

        internal static MarkupString ToTwitterMarkupString(this string twitterHandle) =>
            new MarkupString(twitterHandle.ToTwitterUrl());

        public static string ToMvpUrl(this Person person) =>
            $"<a href='https://mvp.microsoft.com/en-us/PublicProfile/{person.MicrosoftMvpId}' target='_blank' aria-label='MVP link for {person.FirstName} {person.LastName}'>MVP 🏅</a>";

        public static IDictionary<string, string> ToShowTags(this DocsShow show) =>
            show.Tags
                .Select(tag => (tag: tag == ".NET" ? ".NET 💬" : tag, color: ""))
                .Concat(
                    show.Guests
                        .Where(p => p.IsMicrosoftMvp)
                        .Select(p => (tag: p.ToMvpUrl(), color: "badge-mvp")))
                .Concat(
                    show.Guests
                        .Where(p => p.IsBlueBadge)
                        .Select(p => (tag: "MSFT 🏢", color: "badge-msft")))
                .ToDictionary(t => t.tag, t => t.color);

        public static string ToSocialText(this DocsShow show)
        {
            var builder = new StringBuilder();
            builder.AppendLine($"Tune in to The .NET Docs Show: {show.Title} - {show.Guests.First().TwitterHandle}");
            builder.AppendLine();

            var date = show.Date!.Value;
            var utc = date.UtcDateTime;
            builder.AppendLine($"📆 {date:dddd} • {date:MMMM} {date.Day}");
            builder.AppendLine($"⏰ {date:hh:mm tt} US/Central • {utc:HH:mm} UTC");
            builder.AppendLine();

            builder.AppendLine("#TheDotNetDocsShow #dotnet");
            builder.AppendLine();

            return builder.ToString();
        }
    }
}
