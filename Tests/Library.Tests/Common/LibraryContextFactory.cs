using System;
using Microsoft.EntityFrameworkCore;
using Library.Domain.Entities;
using Library.Data.Context;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;

namespace Library.Tests.Common
{
    public class LibraryContextFactory
    {
        public static Guid AuthorAId = Guid.NewGuid();
        public static Guid AuthorBId = Guid.NewGuid();
        public static Guid BookIdForDelete = Guid.NewGuid();
        public static Guid BookIdForUpdate = Guid.NewGuid();

        public static LibraryDbContext CreateInMemory()
        {
            var options = new DbContextOptionsBuilder<LibraryDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new LibraryDbContext(options);
            context.Authors.AddRange(
                new Author
                {
                    Id = AuthorAId,
                    Name = "Author One",
                    Birthday = new DateTime(1970, 1, 1),
                    Country = "Country One",
                    Books = new List<Book>
                    {
                        new Book
                        {
                            Id = Guid.Parse("77777777-7777-7777-7777-777777777777"),
                            Name = "Book 1",
                            ISBN = "1234567890",
                            Description = "Description 1",
                            StartRentDateTime = DateTime.Today.AddDays(-10),
                            EndRentDateTime = DateTime.Today.AddDays(-5),
                            ImageFileName = "image1.jpg"
                        }
                    }
                },
                new Author
                {
                    Id = AuthorBId,
                    Name = "Author Two",
                    Birthday = new DateTime(1980, 5, 15),
                    Country = "Country Two",
                    Books = new List<Book>
                    {
                        new Book
                        {
                            Id = Guid.Parse("88888888-8888-8888-8888-888888888888"),
                            Name = "Book 2",
                            ISBN = "0987654321",
                            Description = "Description 2",
                            StartRentDateTime = DateTime.Today.AddDays(-20),
                            EndRentDateTime = DateTime.Today.AddDays(-10),
                            ImageFileName = "image2.jpg"
                        },
                        new Book
                        {
                            Id = BookIdForDelete,
                            Name = "Book 3",
                            ISBN = "1111111111",
                            Description = "Description 3",
                            StartRentDateTime = DateTime.Today.AddDays(-15),
                            EndRentDateTime = DateTime.Today.AddDays(-7),
                            ImageFileName = "image3.jpg"
                        }
                    }
                }
            );

            context.Users.AddRange(
                new User
                {
                    Id = 1,
                    UserName = "user1",
                    FirstName = "John",
                    LastName = "Doe",
                    MiddleName = "M",
                    RefreshToken = "token1",
                    RefreshTokenExpiryTime = DateTime.Now.AddDays(5),
                    Books = new List<Book>
                    {
                        new Book
                        {
                            Id = BookIdForUpdate,
                            Name = "Book 4",
                            ISBN = "2222222222",
                            Description = "Description 4",
                            StartRentDateTime = DateTime.Today.AddDays(-12),
                            EndRentDateTime = DateTime.Today.AddDays(-2),
                            ImageFileName = "image4.jpg"
                        }
                    },
                    Massages = new List<Massage>
                    {
                        new Massage
                        {
                            Id = Guid.Parse("88888888-8888-8888-8888-888888888882"),
                            Desription = "Massage 1",
                            DepartureTime = DateTime.Now.AddHours(-1)
                        }
                    }
                },
                new User
                {
                    Id = 2,
                    UserName = "user2",
                    FirstName = "Jane",
                    LastName = "Smith",
                    MiddleName = "A",
                    RefreshToken = "token2",
                    RefreshTokenExpiryTime = DateTime.Now.AddDays(7),
                    Books = new List<Book>(),
                    Massages = new List<Massage>
                    {
                    }
                }
            );

            context.SaveChanges();
            return context;
        }

        public static void Destroy(LibraryDbContext context)
        {
            context.Database.EnsureDeleted();
            context.Dispose();
        }
    }
}