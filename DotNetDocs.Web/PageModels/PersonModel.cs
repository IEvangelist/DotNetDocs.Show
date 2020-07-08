using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DotNetDocs.Web.PageModels
{
    public class PersonModel
    {
        [Required(ErrorMessage = "A first name is required")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "A last name is required")]
        public string LastName { get; set; } = null!;

        [Required(ErrorMessage = "A valid email address is required"), DataType(DataType.EmailAddress), EmailAddress]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "A valid twitter handle is required (include @)")]
        public string TwitterHandle { get; set; } = null!;

        public string ImageUrl { get; set; } = null!;

        public bool IsBlueBadge { get; set; }

        public int? MicrosoftMvpId { get; set; } = null;
    }
}
