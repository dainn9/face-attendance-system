using BuildingBlocks.Security.Internal;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using user_service.API.Contracts;
using user_service.Application.Features.Users.Commands.CreateUser;

namespace user_service.API.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = InternalAuthenticationHandler.SchemeName)]
    [Route("api/v1/internal/users")]
    public class InternalUsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InternalUsersController(IMediator mediator) => _mediator = mediator;

        // POST: api/v1/internal/users
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            var command = new CreateUserCommand(
                request.UserId,
                request.UserCode,
                request.FullName,
                request.Gender,
                request.DateOfBirth,
                request.Email,
                request.Role,
                request.FacultyId,
                request.MajorId
            );

            await _mediator.Send(command);
            return NoContent();
        }

        // // POST: api/v1/internal/users/validate-profile
        // [HttpPost("validate-profile")]
        // public async Task<IActionResult> ValidateUserProfile([FromBody] ValidateUserProfileRequest request)
        // {
        //     var command = new ValidateUserProfileCommand(
        //         request.Role,
        //         request.StudentCode,
        //         request.ClassCode,
        //         request.FacultyCode
        //     );

        //     await _mediator.Send(command);
        //     return NoContent();
        // }
    }
}