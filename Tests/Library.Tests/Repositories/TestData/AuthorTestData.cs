using Library.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Tests.Common
{
    public static class AuthorTestData
    {
        public static Author CreateTestAuthor(Guid? id = null)
        {
            return new Author
            {
                Id = id ?? Guid.NewGuid(),
                Name = "Test Author",
                Birthday = DateTime.MinValue,
                Country = "Belarus"
            };
        }

        public static Author CreateUpdatedAuthor(Guid? id = null)
        {
            return new Author
            {
                Id = id ?? Guid.NewGuid(),
                Name = "Updated Author",
                Birthday = new DateTime(1990, 1, 1),
                Country = "Netherlands"
            };
        }
    }
}
