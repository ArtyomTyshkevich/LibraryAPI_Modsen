
using Microsoft.AspNetCore.Identity;

namespace Library.Domain.Entities
{
    public class User : IdentityUser<long>
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? MiddleName { get; set; }
        public List<Book> Books { get; set; } = new List<Book>();
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}
