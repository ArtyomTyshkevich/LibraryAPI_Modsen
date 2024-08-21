using Library.Domain.Entities;

namespace Library.Domain.DTOs
{
    public class UserDTO
    {
        public long Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? MiddleName { get; set; }
        public string Email { get; set; } = null!;
        public List<Book> Books { get; set; } = new List<Book>();
    }
}
