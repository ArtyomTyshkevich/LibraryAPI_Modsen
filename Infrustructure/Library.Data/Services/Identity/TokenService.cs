using System.IdentityModel.Tokens.Jwt;
using Library.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Library.Domain.Extensions;
using Microsoft.Extensions.Configuration;
using Library.Application.Interfaces;


namespace Library.Domain.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string CreateToken(User user, List<IdentityRole<long>> roles)
    {
        var token = user
            .CreateClaims(roles)
            .CreateJwtToken(_configuration);
        var tokenHandler = new JwtSecurityTokenHandler();
        
        return tokenHandler.WriteToken(token);
    }
}