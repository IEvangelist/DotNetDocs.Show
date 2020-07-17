using System.ComponentModel.DataAnnotations;

namespace DotNetDocs.Web.PageModels
{
    public class ShowIdeaModel
    {
        [Required(ErrorMessage = "You're pitching a show idea, you need to provide something.")]
        public string Idea { get; set; } = null!;

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        [Required(ErrorMessage = "A valid email address is required"), DataType(DataType.EmailAddress), EmailAddress]
        public string Email { get; set; } = null!;

        public string TwitterHandle { get; set; } = null!;
    }
}
