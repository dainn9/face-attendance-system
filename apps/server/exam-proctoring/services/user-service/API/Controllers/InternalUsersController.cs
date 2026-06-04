using BuildingBlocks.Results;
using BuildingBlocks.Security.Internal;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using user_service.API.Contracts;
using user_service.Application.Contracts;
using user_service.Application.Features.Users.Commands.CreateUser;
using user_service.Application.Features.Users.Queries.GetLecturersByIds;
using user_service.Application.Features.Users.Queries.GetUserPaged;

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

        // GET: api/v1/users?page=1&pageSize=10&searchQuery=John&role=Admin&facultyId=123e4567-e89b-12d3-a456-426614174000
        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] GetUserPagedRequest request, CancellationToken cancellationToken)
        {
            var query = new GetUserPagedQuery(
                request.Page,
                request.PageSize,
                request.SearchQuery,
                request.Role,
                request.FacultyId
            );

            var result = await _mediator.Send(query, cancellationToken);

            return Ok(new ApiResponse<PagedResult<UserPagedDto>>
            {
                Success = true,
                Message = "Users retrieved successfully",
                Data = result
            });
        }

        // POST: api/v1/internal/users/get-lecturers-by-ids
        [HttpPost("get-lecturers-by-ids")]
        public async Task<IActionResult> GetLecturersByIds([FromBody] IReadOnlyList<Guid> userIds, CancellationToken cancellationToken)
        {
            var query = new GetLecturersByIdsQuery(userIds);
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
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