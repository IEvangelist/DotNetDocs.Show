namespace DotNetDocs.Services.Models
{
    public class Person
    {
        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string TwitterHandle { get; set; } = null!;

        public bool IsBlueBadge { get; set; }

        public bool IsMicrosoftMvp => MicrosoftMvpId.HasValue;

        /// <summary>
        /// https://mvp.microsoft.com/en-us/PublicProfile/{id}
        /// </summary>
        public int? MicrosoftMvpId { get; set; } = null;

        public static readonly Person Cam = new Person
        {
            FirstName = "Cam",
            LastName = "Soper",
            Email = "cam.soper@microsoft.com",
            TwitterHandle = "@camsoper",
            IsBlueBadge = true
        };

        public static readonly Person Scott = new Person
        {
            FirstName = "Scott",
            LastName = "Addie",
            Email = "scott.addie@microsoft.com",
            TwitterHandle = "@scott_addie",
            IsBlueBadge = true
        };

        public static readonly Person David = new Person
        {
            FirstName = "David",
            LastName = "Pine",
            Email = "david.pine@microsoft.com",
            TwitterHandle = "@davidpine7",
            IsBlueBadge = true
        };

        public static readonly Person DotNetDocs = new Person
        {
            FirstName = ".NET",
            LastName = "Docs"
        };
    }
}
