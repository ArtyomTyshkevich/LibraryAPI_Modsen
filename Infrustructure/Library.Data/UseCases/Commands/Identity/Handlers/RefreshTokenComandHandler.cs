using Library.Application.Exceptions;
using Library.Domain.Entities.Identity;
using Library.Domain.Entities;
using Library.Domain.Extensions;
using MediatR;
using Microsoft.Extensions.Configuration;
using Library.Application.Interfaces;
using Library.Data.Context;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace Library.Data.UseCases.Commands.Identity.Handlers
{
    public class RefreshTokenComandHandler : IRequestHandler<RefreshTokenCommand, TokenModel>
    {
        private readonly UserManager<User> _userManager;
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;

        public RefreshTokenComandHandler(IMediator mediator, LibraryDbContext context, UserManager<User> userManager, IConfiguration configuration, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _mediator = mediator;
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<TokenModel> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var principal = _configuration.GetPrincipalFromExpiredToken(request.tokenModel.AccessToken);
            if (principal == null) throw new TokenInvalidException();

            var user = await GetUserAndValidateRefreshToken(principal, request.tokenModel.RefreshToken!, cancellationToken);

            var newAccessToken = _configuration.CreateToken(principal.Claims.ToList());
            var newRefreshToken = _configuration.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            await _userManager.UpdateAsync(user);

            return new TokenModel { AccessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken), RefreshToken = newRefreshToken };
        }

        private async Task<User> GetUserAndValidateRefreshToken(ClaimsPrincipal principal, string refreshToken, CancellationToken cancellationToken)
        {
            var username = principal.Identity!.Name;
            var user = await _userManager.FindByNameAsync(username!);

            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                throw new RefreshTokenInvalidException();
            }

            return user;
        }

    }
}