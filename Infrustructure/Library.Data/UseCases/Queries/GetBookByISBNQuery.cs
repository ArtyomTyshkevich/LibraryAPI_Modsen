using Library.Application.DTOs;
using MediatR;

namespace Library.Data.UseCases.Commands
{

    public class GetBookByISBNQuery : IRequest<BookDTO>
    {
        public string ISBN { get; set; }
    }
}