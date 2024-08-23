

using Library.Domain.Entities;

namespace Library.Domain.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse> Login(LoginUser user);
        Task<LoginResponse> RefreshToken(RefreshTokenModel model);
        Task<bool> RegisterUser(LoginUser user);
    }
}