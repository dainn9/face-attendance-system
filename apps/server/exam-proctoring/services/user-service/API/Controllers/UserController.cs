using BuildingBlocks.Extensions;
using BuildingBlocks.Results;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using user_service.Application.Contracts;
using user_service.Application.Features.Users.Queries.GetUserProfile;

namespace user_service.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/users")]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;
        public UserController(IMediator mediator) => _mediator = mediator;

        [HttpGet("profile")]
        public async Task<IActionResult> GetUserProfile()
        {
            var userId = User.GetUserId();

            var query = new GetUserProfileQuery(userId);
            var result = await _mediator.Send(query);
            return Ok(new ApiResponse<UserDto>
            {
                Success = true,
                Message = "User profile retrieved successfully",
                Data = result
            });
        }
    }
}