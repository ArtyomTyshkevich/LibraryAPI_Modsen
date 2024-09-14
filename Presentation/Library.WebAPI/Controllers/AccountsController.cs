using Library.Application.Interfaces;
using Library.Domain.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library.WebAPI.Controllers
{
    [ApiController]
    [Route("accounts")]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Authenticate([FromBody] AuthRequest request, CancellationToken cancellationToken)
        {
            var response = await _accountService.Authenticate(request, cancellationToken);
            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
        {
            var response = await _accountService.Register(request, cancellationToken);
            return Ok(response);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenModel tokenModel, CancellationToken cancellationToken)
        {
            var newTokens = await _accountService.RefreshToken(tokenModel, cancellationToken);
            return Ok(newTokens);
        }

        [Authorize]
        [HttpPost("revoke/{username}")]
        public async Task<IActionResult> Revoke(string username, CancellationToken cancellationToken)
        {
            await _accountService.Revoke(username, cancellationToken);
            return Ok();
        }

        [HttpPost("revoke-all")]
        [Authorize]
        public async Task<IActionResult> RevokeAll(CancellationToken cancellationToken)
        {
            await _accountService.RevokeAll(cancellationToken);
            return Ok();
        }
    }
}
