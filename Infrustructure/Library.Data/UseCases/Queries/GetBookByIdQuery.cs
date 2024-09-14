using Library.Application.DTOs;
using MediatR;

namespace Library.Data.UseCases.Queries
{
    public class GetBookByIdQuery : IRequest<BookDTO>
    {
        public Guid BookId { get; set; }
    }
}
