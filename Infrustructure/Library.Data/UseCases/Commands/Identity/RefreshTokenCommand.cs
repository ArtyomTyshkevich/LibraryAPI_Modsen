using MediatR;
using Library.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Library.Domain.Entities.Identity;

namespace Library.Data.UseCases.Commands.Identity
{
    public class RefreshTokenCommand : IRequest<TokenModel>
    {
        public TokenModel tokenModel { get; set; }
        public List<IdentityRole<long>> Roles { get; set; }
    }
}