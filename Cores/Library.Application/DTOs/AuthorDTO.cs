
namespace Library.Application.DTOs
{
    public class AuthorDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public DateTime Birthday { get; set; }
        public string Country { get; set; } = "";
    }
}
