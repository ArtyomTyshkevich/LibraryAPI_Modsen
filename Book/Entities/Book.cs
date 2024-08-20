using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Domain.Entities
{
    public class Book
    {
        public Guid Id { get; set; }
        public string ISBN { get; set; } = "";
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public Author Author { get; set; } = new Author();
        public DateTime StartRentDateTime { get; set; }
        public DateTime EndRentDateTime { get; set; }
        public string? ImageFileName { get; set; } = null;
    }
}
