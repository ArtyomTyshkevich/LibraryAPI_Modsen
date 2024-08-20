using Library.Domain.Entities;
using Microsoft.AspNetCore.Identity;


namespace Library.Domain.Services;

public interface ITokenService
{
    string CreateToken(User user, List<IdentityRole<long>> role);
}