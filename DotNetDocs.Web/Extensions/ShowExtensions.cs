using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Encodings.Web;
using DotNetDocs.Repository;
using DotNetDocs.Services.Models;
using Microsoft.AspNetCore.Components;

namespace DotNetDocs.Web.Extensions
{
    static class ShowExtensions
    {
        internal static string AddToGoogleCalendar(this DocsShow show)
        {
            // Reference: http://stackoverflow.com/a/21653600/22941
            var centralTimeZone = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")
                : TimeZoneInfo.FindSystemTimeZoneById("America/Matamoros");

            var date = new DateTimeWithZone(show.Date!.Value, centralTimeZone).UniversalTime;

            var from = UrlEncoder.Default.Encode($"{date:yyyyMMddTHHmmssZ}");
            var to = UrlEncoder.Default.Encode($"{date.AddHours(1):yyyyMMddTHHmmssZ}");
            var text = UrlEncoder.Default.Encode($"The .NET docs show - live with {show.Guests.ToCommaSeparatedString(false)}.");
            var location = UrlEncoder.Default.Encode("https://www.twitch.tv/thedotnetdocs");

            return $"https://www.google.com/calendar/render?action=TEMPLATE&text={text}&dates={from}/{to}&details={location}&location={location}&sf=true&output=xml";
        }

        internal static MarkupString ToGeneralDescription(this DocsShow show) =>
            new MarkupString($"Join {show.Guests.ToCommaSeparatedString()}, along with hosts {show.Hosts.ToCommaSeparatedString(false)} for a captivating conversation about .NET.");

        internal static string ToCommaSeparatedString(this IEnumerable<Person> persons, bool includeTwitter = true) =>
            string.Join(", ", persons.Select(p => p.ToPrintFriendlyName(includeTwitter)));

        internal static string ToPrintFriendlyName(this Person person, bool includeTwitter = true)
        {
            string? twitterHandle = person.TwitterHandle is null || !includeTwitter ? null : $" ({person.TwitterHandle.ToTwitterUrl()})";
            return $"{person.FirstName} {person.LastName}{twitterHandle}";
        }

        internal static string ToTwitterUrl(this string twitterHandle)
        {
            string? name = twitterHandle.Substring(1);
            return $"<a href=\"https://twitter.com/{name}\" target=\"_blank\">{twitterHandle}</a>";
        }
    }
}
