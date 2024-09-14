using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Library.Application.Exceptions;
using Library.Application.Interfaces;
using Library.Data.Context;
using Library.Domain.Entities;
using Library.Domain.Entities.Identity;
using Library.Domain.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Library.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<User> _userManager;
        private readonly LibraryDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;

        public AccountService(ITokenService tokenService, LibraryDbContext context, UserManager<User> userManager, IConfiguration configuration)
        {
            _tokenService = tokenService;
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<AuthResponse> Authenticate(AuthRequest request, CancellationToken cancellationToken)
        {
            var user = await ValidateUserCredentials(request, cancellationToken);
            var roleIds = await _context.UserRoles.Where(r => r.UserId == user.Id).Select(x => x.RoleId).ToListAsync(cancellationToken);
            var roles = await _context.Roles.Where(x => roleIds.Contains(x.Id)).ToListAsync(cancellationToken);
            var accessToken = _tokenService.CreateToken(user, roles);
            UpdateUserTokenAndExpiry(user);

            await _context.SaveChangesAsync(cancellationToken);

            return new AuthResponse
            {
                Username = user.UserName!,
                Email = user.Email!,
                Token = accessToken,
                RefreshToken = user.RefreshToken
            };
        }

        private async Task<User> ValidateUserCredentials(AuthRequest request, CancellationToken cancellationToken)
        {
            var managedUser = await _userManager.FindByEmailAsync(request.Email);
            if (managedUser == null || !await _userManager.CheckPasswordAsync(managedUser, request.Password))
            {
                throw new BadCredentialsException();
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);
            if (user is null) throw new UserNotFoundException(request.Email);

            return user;
        }

        private void UpdateUserTokenAndExpiry(User user)
        {
            user.RefreshToken = _configuration.GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_configuration.GetSection("Jwt:RefreshTokenValidityInDays").Get<int>());
        }

        public async Task<AuthResponse> Register(RegisterRequest request, CancellationToken cancellationToken)
        {
            var user = CreateRegisterUser(request);
            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded) throw new UserCreationFailedException();
            await _userManager.AddToRoleAsync(user, RoleConsts.User);
            return await Authenticate(new AuthRequest { Email = request.Email, Password = request.Password }, cancellationToken);
        }

        private User CreateRegisterUser(RegisterRequest request)
        {
            return new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                MiddleName = request.MiddleName,
                Email = request.Email,
                UserName = request.Email
            };
        }

        public async Task<TokenModel> RefreshToken(TokenModel tokenModel, CancellationToken cancellationToken)
        {
            var principal = _configuration.GetPrincipalFromExpiredToken(tokenModel.AccessToken);
            if (principal == null) throw new TokenInvalidException();

            var user = await GetUserAndValidateRefreshToken(principal, tokenModel.RefreshToken!, cancellationToken);

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

        public async Task Revoke(string username, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null) throw new ArgumentException("Invalid user");

            user.RefreshToken = null;
            await _userManager.UpdateAsync(user);
        }

        public async Task RevokeAll(CancellationToken cancellationToken)
        {
            var users = await _userManager.Users.ToListAsync(cancellationToken);
            foreach (var user in users)
            {
                user.RefreshToken = null;
                await _userManager.UpdateAsync(user);
            }
        }
    }
}
