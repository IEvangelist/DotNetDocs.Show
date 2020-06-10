using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DotNetDocs.Web.PageModels
{
    public class ShowModel
    {
        public string Id { get; set; } = null!;

        [Required, DataType(DataType.DateTime)]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "A title is required.")]
        public string Title { get; set; } = null!;

        [Required, Url]
        public string Url { get; set; } = null!;

        public string ShowImage { get; set; } = null!;

        public int? VideoId { get; set; }

        public IEnumerable<PersonModel> Guests { get; set; } = null!;

        public IEnumerable<PersonModel> Hosts { get; set; } = null!;

        public IEnumerable<string> Tags { get; set; } = null!;
    }
}
