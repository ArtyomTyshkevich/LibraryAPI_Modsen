using Library.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Library.Data.UseCases.Commands.BooksCommands
{
    public class CreateFormFileFromBookDTOCommand : IRequest<IFormFile?>
    {
        public BookDTO BookDTO { get; set; }
    }
}
