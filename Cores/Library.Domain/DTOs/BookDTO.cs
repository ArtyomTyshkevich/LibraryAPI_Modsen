using Library.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Library.Domain.DTOs
{
    public class BookDTO
    {
        public Guid Id { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
}