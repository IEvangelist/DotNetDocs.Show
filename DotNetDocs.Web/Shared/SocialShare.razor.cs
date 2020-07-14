using System;
using DotNetDocs.Services.Models;
using DotNetDocs.Web.Extensions;
using Microsoft.AspNetCore.Components;

using static System.Web.HttpUtility;

namespace DotNetDocs.Web.Shared
{
    public partial class SocialShare
    {
        [Parameter]
        public DocsShow Show { get; set; } = null!;

        static readonly string SiteUrlEncoded = UrlEncode("https://dotnetdocs.dev/");
        static readonly string TagsUrlEncoded = UrlDecode("#TheDotNetDocsShow,#DotNet");

        string BuildSocialUrl(SocialKind kind) => kind switch
        {
            SocialKind.Twitter => ToTwitterUrl,
            SocialKind.LinkedIn => ToLinkedInUrl,
            SocialKind.Reddit => ToRedditUrl,
            SocialKind.Email => ToEmailUrl,

            _ => throw new Exception("Not sure how this happened.")
        };

        string ToTwitterUrl =>
            Show.IsInFuture
                ? $"https://twitter.com/intent/tweet?url={SiteUrlEncoded}&text={UrlEncode(Show.ToSocialText())}&hashtags={TagsUrlEncoded}&via=dotnetdocsshow"
                : $"https://twitter.com/intent/tweet?url={UrlEncode(Show.Url)}&text={UrlEncode(Show.Title)}&hashtags={TagsUrlEncoded}&via=dotnetdocsshow";

        string ToLinkedInUrl =>
            Show.IsInFuture
                ? $"https://www.linkedin.com/sharing/share-offsite?mini=true&url={SiteUrlEncoded}&title={UrlEncode(Show.Title)}&summary={UrlEncode(Show.ToSocialText())}&tags={TagsUrlEncoded}"
                : $"https://www.linkedin.com/sharing/share-offsite?mini=true&url={UrlEncode(Show.Url)}&title={UrlEncode(Show.Title)}&summary={UrlEncode(Show.ToSocialText())}&tags={TagsUrlEncoded}";

        string ToRedditUrl =>
            Show.IsInFuture
                ? $"http://www.reddit.com/r/dotnet/submit?kind=self&title={UrlEncode(Show.Title)}&selftext=true&text={UrlEncode(Show.ToSocialText())}"
                : $"http://www.reddit.com/r/dotnet/submit?kind=link&title={UrlEncode(Show.Title)}&url={UrlEncode(Show.Url)}";

        string ToEmailUrl =>
            Show.IsInFuture
                ? $"mailto:?subject={Show.Title}&body={Show.ToSocialText().Replace("\n", "%0D%0A")}"
                : $"mailto:?subject={Show.Title}&body={UrlEncode($"Watch this: {Show.Url}")}";
    }

    enum SocialKind
    {
        Twitter,
        LinkedIn,
        Reddit,
        Facebook,
        Email
    }
}
