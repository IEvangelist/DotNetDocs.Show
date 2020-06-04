using System.ComponentModel.DataAnnotations;

namespace DotNetDocs.Web.PageModels
{
    public class PersonModel
    {
        [Required]
        public string FirstName { get; set; } = null!;

        [Required]
        public string LastName { get; set; } = null!;

        [Required, DataType(DataType.EmailAddress), EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string TwitterHandle { get; set; } = null!;

        public bool IsBlueBadge { get; set; }
    }
}
