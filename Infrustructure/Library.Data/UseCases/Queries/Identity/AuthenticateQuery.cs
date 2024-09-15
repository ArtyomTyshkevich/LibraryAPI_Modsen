using MediatR;
using Microsoft.AspNetCore.Identity;
using Library.Domain.Entities.Identity;

namespace Library.Data.UseCases.Queries.Identity
{
    public class AuthenticateQuery : IRequest<AuthResponse>
    {
        public AuthRequest AuthRequest { get; set; }
        public List<IdentityRole<long>> Roles { get; set; }
    }
}