﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Azure.CosmosRepository;

namespace DotNetDocs.Services.Models
{
    [DebuggerDisplay("Date = {Date}, Title = {Title}, Published = {IsPublished}")]
    public class DocsShow : Item
    {
        public DateTimeOffset? Date { get; set; }

        public bool IsPlaceholder { get; }

        public bool IsScheduled => Date.HasValue;

        public bool IsInFuture => Date > DateTimeOffset.Now;

        public bool IsNew => !IsInFuture && Date.HasValue && (DateTimeOffset.Now - Date.Value).TotalDays <= 14;

        public bool IsPublished { get; set; } = true;

        public bool IsCalendarInviteSent { get; set; }

        public string? CalendarInviteId { get; set; }

        public string GuestStreamUrl { get; set; } = null!;

        public IEnumerable<Person> Guests { get; set; } = new Person[] { Person.DotNetDocs };

        public IEnumerable<Person> Hosts { get; set; } = Person.RandomHosts.ToArray();

        public IEnumerable<string> Tags { get; set; } = new string[] { ".NET" };

        public string Title { get; set; } = "The .NET docs show";

        public string Url { get; set; } = "https://www.twitch.tv/visualstudio";

        public string TldrUrl { get; set; } = null!;

        public string ShowImage { get; set; } = null!;

        public int? VideoId => this.GetVideoId();

        public DocsShow() { }

        private DocsShow(DateTimeOffset showDate) =>
            (IsPlaceholder, IsPublished, Date) = (true, false, showDate);

        public static DocsShow CreatePlaceholder(DateTimeOffset showDate) =>
            new DocsShow(showDate);
    }
}
