namespace DotNetDocs.Models
{
    public class Person
    {
        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string TwitterHandle { get; set; } = null!;

        public static readonly Person Cam = new Person
        {
            FirstName = "Cam",
            LastName = "Soper",
            Email = "cam.soper@microsoft.com",
            TwitterHandle = "@camsoper"
        };

        public static readonly Person Scott = new Person
        {
            FirstName = "Scott",
            LastName = "Addie",
            Email = "scott.addie@microsoft.com",
            TwitterHandle = "@scott_addie"
        };

        public static readonly Person David = new Person
        {
            FirstName = "David",
            LastName = "Pine",
            Email = "david.pine@microsoft.com",
            TwitterHandle = "@davidpine7"
        };
    }
}