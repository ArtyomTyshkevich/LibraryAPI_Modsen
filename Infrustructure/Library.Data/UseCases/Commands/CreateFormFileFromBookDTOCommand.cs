using Library.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Library.Data.UseCases.Commands
{
    public class CreateFormFileFromBookDTOCommand : IRequest<IFormFile?>
    {
        public BookDTO BookDTO { get; set; }
    }
}
