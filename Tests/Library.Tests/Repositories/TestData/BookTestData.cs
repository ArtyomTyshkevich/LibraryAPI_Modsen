using Library.Domain.Entities;

namespace Library.Tests.Common
{
    public static class BookTestData
    {
        public static Book CreateTestBook()
        {
            return new Book
            {
                Id = Guid.NewGuid(),
                Name = "Test Book",
                ISBN = "1234567890",
                Description = "Test Description",
                StartRentDateTime = DateTime.Now.AddDays(-10),
                EndRentDateTime = DateTime.Now.AddDays(-5),
                ImageFileName = "test_image.jpg",
                Author = new Author
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Author",
                    Birthday = new DateTime(1970, 1, 1),
                    Country = "Test Country"
                }
            };
        }
    }
}
