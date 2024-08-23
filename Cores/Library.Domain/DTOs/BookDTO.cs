using Library.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Library.Domain.DTOs
{
    public class BookDTO
    {
        public Guid Id { get; set; }
        public string? ISBN { get; set; } = "";
        public string? Name { get; set; } = "";
        public string? Description { get; set; } = "";
        public Author? Author { get; set; } = null;
        public IFormFile? ImageFile { get; set; }
        public string? ImageFileName { get; set; } = null;

    }
}