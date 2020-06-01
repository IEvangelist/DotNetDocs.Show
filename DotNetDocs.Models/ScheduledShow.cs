using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNetDocs.Models
{
    public class ScheduledShow
    {
        public DateTime Date { get; set; }

        public IEnumerable<Person> Guests { get; set; } = Enumerable.Empty<Person>();

        public IEnumerable<Person> Hosts { get; set; } = new Person[] { Person.Cam, Person.Scott, Person.David };

        public IEnumerable<string> Tags { get; set; } = new string[] { ".NET" };

        public string Title { get; set; } = "The .NET docs show";

        public string Url { get; set; } = null!;

        public string ShowImage { get; set; } = null!;
    }
}
