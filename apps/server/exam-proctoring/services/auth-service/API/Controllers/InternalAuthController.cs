using auth_service.API.Contracts;
using auth_service.Application.Features.Auth.Commands.CreateAccount;
using auth_service.Application.Features.Auth.Commands.DeleteAccount;
using BuildingBlocks.Security.Internal;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace auth_service.API.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = InternalAuthenticationHandler.SchemeName)]
    [Route("api/v1/internal/auth/accounts")]
    public class InternalAuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InternalAuthController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequest request)
        {
            var command = new CreateAccountCommand(
                request.Email,
                request.Password,
                request.UserRole
            );

            var userId = await _mediator.Send(command);
            return Ok(new { UserId = userId });
        }

        [HttpDelete("{userId:guid}")]
        public async Task<IActionResult> DeleteAccount([FromRoute] Guid userId)
        {
            var command = new DeleteAccountCommand(userId);
            await _mediator.Send(command);
            return NoContent();
        }
    }
}