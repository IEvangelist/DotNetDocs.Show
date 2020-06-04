using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DotNetDocs.Web.PageModels
{
    public class ShowModel
    {
        [Required, DataType(DataType.DateTime)]
        public DateTime Date { get; set; }

        [Required]
        public string Title { get; set; }

        [Required, Url]
        public string Url { get; set; }

        [Required]
        public string ShowImage { get; set; }

        public IEnumerable<PersonModel> Guests { get; set; }

        public IEnumerable<PersonModel> Hosts { get; set; }

        public IEnumerable<string> Tags { get; set; }
    }
}
