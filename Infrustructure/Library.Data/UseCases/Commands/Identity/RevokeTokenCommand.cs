using MediatR;

namespace Library.Data.UseCases.Commands.Identity
{
    public class RevokeTokenCommand : IRequest<Unit>
    {
        public string Username { get; set; }
    }

    public class RevokeAllTokensCommand : IRequest<Unit>
    {
    }
}