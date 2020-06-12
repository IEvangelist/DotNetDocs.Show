using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using DotNetDocs.Services.Models;
using Microsoft.AspNetCore.Components;

namespace DotNetDocs.Web.Extensions
{
    static class ShowExtensions
    {
        internal static string AddToGoogleCalendar(this DocsShow show)
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
            return $"<a href=\"https://twitter.com/{name}\" target=\"_blank\">{twitterHandle} <span class='oi oi-sm oi-external-link'></span></a>";
        }

        internal static MarkupString ToTwitterMarkupString(this string twitterHandle) =>
            new MarkupString(twitterHandle.ToTwitterUrl());

        internal static string ToMvpUrl(this Person person) =>
            $"<a href='https://mvp.microsoft.com/en-us/PublicProfile/{person.MicrosoftMvpId}' target='_blank'>MVP</a>";
    }
}
