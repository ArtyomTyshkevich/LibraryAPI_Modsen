using Library.Data.Context;
using Library.Domain.Entities;
using Library.Domain.Interfaces;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Library.Data.Consumers
{
    public class BookRentConsumer : IConsumer<Book>
    {
        private readonly LibraryDbContext _libraryDbContext;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPublishEndpoint _publishEndpoint;

        public BookRentConsumer(LibraryDbContext gameDbContext, IUnitOfWork unitOfWork, IPublishEndpoint publishEndpoint)
        {
            _libraryDbContext = gameDbContext;
            _unitOfWork = unitOfWork;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<Book> context)
        {
            var endTime = context.Message.EndRentDateTime;
            var bookСurrent = await _unitOfWork.Books.Get(context.Message.Id);
            if (endTime != bookСurrent.EndRentDateTime)
            {
                endTime = bookСurrent.EndRentDateTime;
            }
            if (endTime == null)
            {

            }
            else if (endTime < DateTime.UtcNow)
            {
                var massage = new Domain.Entities.Massage()
                {
                    DepartureTime = DateTime.UtcNow,
                    Desription = $"You have expired the book with ID: {context.Message.Name}/n" +
                    $"Date and time of the end Rent:{context.Message.EndRentDateTime}/n" +
                    $"Date and time of departure: {DateTime.UtcNow}"

                };
                var user = await _libraryDbContext.Users
                                    .Where(u => u.Books.Any(b => b.Id == context.Message.Id))
                                    .FirstAsync();
                user.Massages.Add(massage);
    
            }
            else if (endTime >= DateTime.UtcNow)
            {
                await _publishEndpoint.Publish(bookСurrent);
            }
            await _libraryDbContext.SaveChangesAsync();
            return;
        }
    }
}
