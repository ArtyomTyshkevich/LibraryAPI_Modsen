using Library.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Library.Data.UseCases.Commands
{
    public class AddImageToBookCommand : IRequest
    {
        public BookDTO BookDTO { get; set; }
    }
}
