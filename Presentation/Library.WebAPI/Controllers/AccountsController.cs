using Library.Data.UseCases.Commands.Identity;
using Library.Data.UseCases.Queries.Identity;
using Library.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library.WebAPI.Controllers
{
    [ApiController]
    [Route("accounts")]
    public class AccountsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AccountsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Authenticate([FromBody] AuthRequest request, CancellationToken cancellationToken)
        {
            var query = new AuthenticateQuery { AuthRequest = request };
            var response = await _mediator.Send(query, cancellationToken);
            return Ok(response);
        }
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
        {
            var query = new RegisterCommand { RegisterRequest = request };
            var response = await _mediator.Send(query, cancellationToken);
            return Ok(response);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenModel tokenModel, CancellationToken cancellationToken)
        {
            var query = new RefreshTokenCommand { tokenModel = tokenModel};
            var newTokens = await _mediator.Send(query, cancellationToken);
            return Ok(newTokens);
        }

        [Authorize]
        [HttpPost("revoke/{username}")]
        public async Task<IActionResult> Revoke(string username, CancellationToken cancellationToken)
        {
            var query = new RevokeTokenCommand { Username = username};
            await _mediator.Send(query, cancellationToken);
            return Ok();
        }

        [HttpPost("revoke-all")]
        [Authorize]
        public async Task<IActionResult> RevokeAll(CancellationToken cancellationToken)
        {
            var query = new RevokeAllTokensCommand { };
            await _mediator.Send(query, cancellationToken);
            return Ok();
        }
    }
}
