using Library.Application.Exceptions;
using Library.Domain.Entities.Identity;
using Library.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Configuration;
using Library.Application.Interfaces;
using Library.Data.Context;
using Microsoft.AspNetCore.Identity;

namespace Library.Data.UseCases.Queries.Identity.Handlers
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponse>
    {
        private readonly UserManager<User> _userManager;
        private readonly IMediator _mediator;

        public RegisterCommandHandler(IMediator mediator, LibraryDbContext context, UserManager<User> userManager)
        {
            _mediator = mediator;
            _userManager = userManager;
        }

        public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var user = CreateRegisterUser(request.RegisterRequest);
            var result = await _userManager.CreateAsync(user, request.RegisterRequest.Password);
            if (!result.Succeeded) throw new UserCreationFailedException();
            await _userManager.AddToRoleAsync(user, RoleConsts.User);
           
            var authQuery = new AuthenticateQuery
            {
                AuthRequest = new AuthRequest
                {
                    Email = request.RegisterRequest.Email,
                    Password = request.RegisterRequest.Password
                }
            };
            return await _mediator.Send(authQuery, cancellationToken);
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

    }
}