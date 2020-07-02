using DotNetDocs.Repository;
using System;
using System.Collections.Generic;

namespace DotNetDocs.Services.Models
{
    public class DocsShow : Document
    {
        public DateTimeOffset? Date { get; set; }

        public bool IsPlaceholder { get; }

        public bool IsScheduled => Date.HasValue;

        public bool IsInFuture => Date > DateTimeOffset.Now;

        public bool IsNew => !IsInFuture && Date.HasValue && (DateTimeOffset.Now - Date.Value).TotalDays <= 14;

        public bool IsPublished { get; set; } = true;

        public bool IsCalendarInviteSent { get; set; }

        public string GuestStreamUrl { get; set; } = null!;

        public IEnumerable<Person> Guests { get; set; } = new Person[] { Person.DotNetDocs };

        public IEnumerable<Person> Hosts { get; set; } = new Person[] { Person.Cam, Person.Scott, Person.David };

        public IEnumerable<string> Tags { get; set; } = new string[] { ".NET" };

        public string Title { get; set; } = "The .NET docs show";

        public string Url { get; set; } = "https://www.twitch.tv/thedotnetdocs";

        public string ShowImage { get; set; } = null!;

        public int? VideoId => this.GetVideoId();

        public DocsShow() { }

        private DocsShow(DateTimeOffset showDate) =>
            (IsPlaceholder, Date) = (true, showDate);

        public static DocsShow CreatePlaceholder(DateTimeOffset showDate) =>
            new DocsShow(showDate);
    }
}
