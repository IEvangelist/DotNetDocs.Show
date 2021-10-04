using System;
using System.Linq;

namespace DotNetDocs.Services.Models
{
    public static class DocsShowExtensions
    {
        public static int? GetVideoId(this DocsShow show)
        {
            if (show is null || show.Url is null)
            {
                return default;
            }

            var uri = new Uri(show.Url);
            string? showId = uri.Segments.LastOrDefault();

            return int.TryParse(showId, out int id) ? id : default;
        }

        public static string ToGuestAndHostEmailString(this DocsShow show)
        {
            var emailString =
                string.Join(";",
                    show.Guests
                        .Select(g => g.Email)
                        .Concat(show.Hosts.Select(h => h.Email))
                        .Concat(new[] { "dotnetdocsshow@microsoft.com" }));

            return emailString;
        }
    }
}
