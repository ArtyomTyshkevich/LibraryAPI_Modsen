using MediatR;
using Library.Application.DTOs;

namespace Library.Data.UseCases.Commands.BooksCommands
{
    public class AddImageToBookCommand : IRequest<Unit>
    {
        public BookDTO BookDTO { get; set; }
    }
}