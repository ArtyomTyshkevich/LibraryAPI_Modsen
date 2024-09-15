using Library.Application.DTOs;
using MediatR;

namespace Library.Data.UseCases.Queries.BooksQueries
{
    public class GetBookByIdQuery : IRequest<BookDTO>
    {
        public Guid BookId { get; set; }
    }
}
