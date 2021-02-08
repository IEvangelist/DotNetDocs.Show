using System;
using System.ComponentModel.DataAnnotations;

namespace DotNetDocs.Web.PageModels
{
    public class RequestModel
    {
        [Required, DataType(DataType.Date)]
        public DateTimeOffset ShowDate { get; set; }

        public string TentativeTitle { get; set; } = null!;

        [Required(ErrorMessage = "You're pitching a show idea, you need to provide something.")]
        public string Idea { get; set; } = null!;

        [Required(ErrorMessage = "A first name is required")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "A last name is required")]
        public string LastName { get; set; } = null!;

        [Required(ErrorMessage = "A valid email address is required"), DataType(DataType.EmailAddress), EmailAddress]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "A valid twitter handle is required (include @)")]
        public string TwitterHandle { get; set; } = null!;

        public string? Pronoun { get; set; } = null!;
    }
}
