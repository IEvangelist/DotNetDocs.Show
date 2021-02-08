using System.Collections.Generic;
using DotNetDocs.Extensions;

namespace DotNetDocs.Services.Models
{
    public class Person
    {
        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string TwitterHandle { get; set; } = null!;

        public string ImageUrl { get; set; } = null!;

        public string? Pronoun { get; set; } = null!;

        public bool IsBlueBadge { get; set; }

        public bool IsMicrosoftMvp => MicrosoftMvpId.HasValue;

        /// <summary>
        /// https://mvp.microsoft.com/en-us/PublicProfile/{id}
        /// </summary>
        public int? MicrosoftMvpId { get; set; } = null;

        public static readonly Person Cecil = new Person
        {
            FirstName = nameof(Cecil),
            LastName = "Phillip",
            Email = "cecil.phillip@microsoft.com",
            TwitterHandle = "@cecilphillip",
            IsBlueBadge = true
        };

        public static readonly Person Luis = new Person
        {
            FirstName = nameof(Luis),
            LastName = "Quintanilla",
            Email = "luis.quintanilla@microsoft.com",
            TwitterHandle = "@ljquintanilla",
            IsBlueBadge = true
        };

        public static readonly Person Maira = new Person
        {
            FirstName = nameof(Maira),
            LastName = "Wenzel",
            Email = "maira.wenzel@microsoft.com",
            TwitterHandle = "@mairacw",
            IsBlueBadge = true
        };

        public static readonly Person Cam = new Person
        {
            FirstName = nameof(Cam),
            LastName = "Soper",
            Email = "cam.soper@microsoft.com",
            TwitterHandle = "@camsoper",
            IsBlueBadge = true
        };

        public static readonly Person Scott = new Person
        {
            FirstName = nameof(Scott),
            LastName = "Addie",
            Email = "scott.addie@microsoft.com",
            TwitterHandle = "@scott_addie",
            IsBlueBadge = true
        };

        public static readonly Person David = new Person
        {
            FirstName = nameof(David),
            LastName = "Pine",
            Email = "david.pine@microsoft.com",
            TwitterHandle = "@davidpine7",
            IsBlueBadge = true
        };

        public static readonly Person DotNetDocs = new Person
        {
            FirstName = ".NET",
            LastName = "Docs",
            Email = "dotnetdocsshow@microsoft.com"
        };

        public static IEnumerable<Person> RandomHosts => new[]
        {
            Cam, Cecil, David, Luis, Maira, Scott
        }.Random(3);
    }
}
