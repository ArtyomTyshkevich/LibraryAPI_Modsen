using Library.Application.DTOs;
using MediatR;

namespace Library.Data.UseCases.Queries.BooksQueries
{

    public class GetBookByISBNQuery : IRequest<BookDTO>
    {
        public string ISBN { get; set; }
    }
}