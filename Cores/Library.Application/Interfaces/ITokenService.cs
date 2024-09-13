using Library.Domain.Entities;
using Microsoft.AspNetCore.Identity;


namespace Library.Application.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(User user, List<IdentityRole<long>> role);
    }
}