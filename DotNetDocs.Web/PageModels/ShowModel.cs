using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NodaTime;
using NodaTime.Text;

namespace DotNetDocs.Web.PageModels
{
    public class ShowModel
    {
        public string Id { get; set; } = null!;

        ZonedDateTime _date;

        public ZonedDateTime Date
        {
            get => _date;
            set
            {
                _date = value;
                DateString = _date.ToString();
            }
        }

        string _dateString = null!;

        public string DateString
        {
            get => _dateString;
            set
            {
                _dateString = value;
                ParseResult<ZonedDateTime>? result =
                    ZonedDateTimePattern.CreateWithCurrentCulture("G", DateTimeZoneProviders.Tzdb)
                                        .Parse(_dateString);
                if (result.Success)
                {
                    _date = result.Value;
                }
            }
        }

        [Required(ErrorMessage = "A show title is required.")]
        public string Title { get; set; } = null!;

        [Required, Url]
        public string Url { get; set; } = null!;

        public string ShowImage { get; set; } = null!;

        public int? VideoId { get; set; }

        public bool IsPublished { get; set; }

        public bool IsCalendarInviteSent { get; set; }

        [Url]
        public string GuestStreamUrl { get; set; } = null!;

        public IEnumerable<PersonModel> Guests { get; set; } = null!;

        public IEnumerable<PersonModel> Hosts { get; set; } = null!;

        public IEnumerable<string> Tags { get; set; } = null!;
    }
}
