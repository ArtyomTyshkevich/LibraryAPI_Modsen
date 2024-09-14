

namespace Library.Domain.Entities
{
    public class Book
    {
        public Guid Id { get; set; }
        public string ISBN { get; set; } = "";
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public Author? Author { get; set; } = null;
        public DateTime? StartRentDateTime { get; set; } = null;
        public DateTime? EndRentDateTime { get; set; } = null;
        public string? ImageFileName { get; set; } = null;
    }
}
    