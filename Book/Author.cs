using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Domain
{
    internal class Author
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public DateOnly Bithday { get; set; }
        public string Country { get; set; } = "";
    }
}
