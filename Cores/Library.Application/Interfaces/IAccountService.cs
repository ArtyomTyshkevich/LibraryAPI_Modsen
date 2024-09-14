using Library.Domain.Entities.Identity;

namespace Library.Application.Interfaces
{
    public interface IAccountService
    {
        Task<AuthResponse> Authenticate(AuthRequest request, CancellationToken cancellationToken);
        Task<AuthResponse> Register(RegisterRequest request, CancellationToken cancellationToken);
        Task<TokenModel> RefreshToken(TokenModel tokenModel, CancellationToken cancellationToken);
        Task Revoke(string username, CancellationToken cancellationToken);
        Task RevokeAll(CancellationToken cancellationToken);
    }
}