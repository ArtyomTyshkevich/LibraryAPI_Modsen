using Library.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Library.Data.UseCases.Commands.Identity.Handlers
{
    public class RevokeTokenCommandHandler : IRequestHandler<RevokeTokenCommand>
    {
        private readonly UserManager<User> _userManager;

        public RevokeTokenCommandHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Unit> Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            user.RefreshToken = null;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                throw new InvalidOperationException("Failed to update user token");
            }

            return Unit.Value;
        }
    }

    public class RevokeAllTokensCommandHandler : IRequestHandler<RevokeAllTokensCommand>
    {
        private readonly UserManager<User> _userManager;

        public RevokeAllTokensCommandHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Unit> Handle(RevokeAllTokensCommand request, CancellationToken cancellationToken)
        {
            var users = await _userManager.Users.ToListAsync(cancellationToken);

            var updateTasks = users.Select(user =>
            {
                user.RefreshToken = null;
                return _userManager.UpdateAsync(user);
            });

            var results = await Task.WhenAll(updateTasks);

            if (results.Any(result => !result.Succeeded))
            {
                throw new InvalidOperationException("Failed to update some user tokens");
            }

            return Unit.Value;
        }
    }
}