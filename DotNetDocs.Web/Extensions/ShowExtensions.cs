using System.Collections.Generic;
using System.Linq;
using DotNetDocs.Services.Models;
using Microsoft.AspNetCore.Components;

namespace DotNetDocs.Web.Extensions
{
    static class ShowExtensions
    {
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
