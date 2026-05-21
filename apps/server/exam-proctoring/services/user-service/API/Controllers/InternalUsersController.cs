using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using user_service.API.Auth;
using user_service.API.Contracts;
using user_service.Application.Features.Users.Commands.CreateUser;

namespace user_service.API.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = InternalAuthenticationHandler.SchemeName)]
    [Route("internal/users")]
    public class InternalUsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InternalUsersController(IMediator mediator) => _mediator = mediator;

        // POST: internal/users
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            var command = new CreateUserCommand(
                request.UserId,
                request.FullName,
                request.Gender,
                request.DateOfBirth,
                request.Email
            );
            await _mediator.Send(command);
            return NoContent();
        }
    }
}