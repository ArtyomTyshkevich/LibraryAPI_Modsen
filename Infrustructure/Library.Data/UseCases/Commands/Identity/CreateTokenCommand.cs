using MediatR;
using Library.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Library.Data.UseCases.Commands.Identity
{
    public class CreateTokenCommand : IRequest<string>
    {
        public User User { get; set; }
        public List<IdentityRole<long>> Roles { get; set; } 
    }
}