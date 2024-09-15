using MediatR;
using Library.Domain.Entities.Identity;

namespace Library.Data.UseCases.Queries.Identity
{
    public class RegisterCommand : IRequest<AuthResponse>
    {
        public RegisterRequest RegisterRequest { get; set; }
    }
}        //public async Task<AuthResponse> Register(RegisterRequest request, CancellationToken cancellationToken)