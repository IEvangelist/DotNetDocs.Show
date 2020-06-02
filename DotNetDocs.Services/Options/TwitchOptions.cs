namespace DotNetDocs.Services.Options
{
    public class TwitchOptions
    {
        public string ClientId { get; set; } = null!;

        public string ClientSecret { get; set; } = null!;

        internal string ToQueryString() => $"client_id={ClientId}&client_secret={ClientSecret}";
    }
}
