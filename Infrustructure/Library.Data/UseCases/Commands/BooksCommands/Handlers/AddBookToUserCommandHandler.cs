using Library.Application.Exceptions;
using Library.Application.Interfaces;

namespace Library.Data.UseCases.Commands.Handlers
{
    using global::Library.Data.UseCases.Commands.BooksCommands;
    using MediatR;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    namespace Library.Data.UseCases.Commands.Handlers
    {
        public class AddBookToUserCommandHandler : IRequestHandler<AddBookToUserCommand, string>
        {
            private readonly IUnitOfWork _unitOfWork;

            public AddBookToUserCommandHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<string> Handle(AddBookToUserCommand request, CancellationToken cancellationToken)
            {
                var user = await _unitOfWork.Users.Get(request.UserId, cancellationToken);
                var book = await _unitOfWork.Books.Get(request.BookId, cancellationToken);

                if (book == null)
                {
                    throw new BookNotFoundException(request.BookId);
                }

                if (book.StartRentDateTime != null && book.EndRentDateTime == null)
                {
                    throw new BookAlreadyRentedException(request.BookId);
                }

                book.StartRentDateTime = DateTime.UtcNow;
                book.EndRentDateTime = DateTime.UtcNow.AddDays(60);
                user!.Books.Add(book);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return "The book was issued successfully";
            }
        }
    }
}
