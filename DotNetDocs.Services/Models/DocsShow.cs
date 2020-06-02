using DotNetDocs.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNetDocs.Services.Models
{
    public class DocsShow : Document
    {
        public DateTime? Date { get; set; }

        public bool IsScheduled => Date.HasValue;

        public bool IsInFuture => Date > DateTimeOffset.Now;

        public bool IsNew => !IsInFuture && Date.HasValue && (DateTimeOffset.Now - Date.Value).TotalDays <= 14;

        public IEnumerable<Person> Guests { get; set; } = new Person[0];

        public IEnumerable<Person> Hosts { get; set; } = new Person[] { Person.Cam, Person.Scott, Person.David };

        public IEnumerable<string> Tags { get; set; } = new string[] { ".NET" };

        public string Title { get; set; } = "The .NET docs show";

        public string Url { get; set; } = null!;

        public string ShowImage { get; set; } = null!;
    }
}
