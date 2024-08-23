

namespace Library.Domain.Entities
{
    public class Author
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public DateTime Birthday { get; set; }
        public string Country { get; set; } = "";
        public List<Book> Books { get; set; } = new List<Book>();
    }
}
